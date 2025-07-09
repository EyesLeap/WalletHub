using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletHub.API.Dtos.Account;
using WalletHub.API.Dtos.Account.Google;
using WalletHub.API.Dtos.AuthToken;
using WalletHub.API.Exceptions;
using WalletHub.API.Exceptions.NotFound;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;

namespace WalletHub.API.Service;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public GoogleAuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<AppUser> FindOrCreateGoogleUserAsync(GoogleUserInfo googleUserInfo)
    {
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleUserInfo.Id);

        if (existingUser is not null)
            return existingUser;

        var userByEmail = await _userManager.FindByEmailAsync(googleUserInfo.Email);
        if (userByEmail is not null)
        {
            userByEmail.GoogleId = googleUserInfo.Id;
            userByEmail.ProfilePictureUrl = googleUserInfo.Picture;
            userByEmail.EmailConfirmed = googleUserInfo.EmailVerified;
            await _userManager.UpdateAsync(userByEmail);
            return userByEmail;
        }

        var username = await GenerateUniqueUsernameAsync(googleUserInfo.Email);

        var newUser = new AppUser
        {
            UserName = username,
            Email = googleUserInfo.Email,
            EmailConfirmed = googleUserInfo.EmailVerified,
            GoogleId = googleUserInfo.Id,
            ProfilePictureUrl = googleUserInfo.Picture,
            Provider = "google"
        };

        var result = await _userManager.CreateAsync(newUser);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new WalletHubException($"Failed to create Google user: {errors}");
        }

        await _userManager.AddToRoleAsync(newUser, "User");

        return newUser;
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(GoogleOAuthDto googleOAuthDto)
    {
        var googleUserInfo = await ValidateGoogleTokenAsync(googleOAuthDto.IdToken);

        if (googleUserInfo is null)
            throw new UnauthorizedException("Invalid Google token.");

        var user = await FindOrCreateGoogleUserAsync(googleUserInfo);

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var tokenResponse = await _tokenService.CreateTokenResponse(user);
        await SaveRefreshTokenAsync(user, tokenResponse.RefreshToken);

        return new AuthResponseDto
        {
            UserName = user.UserName,
            Email = user.Email,
            TokenResponse = tokenResponse
        };
    }

    public async Task<GoogleUserInfo> ValidateGoogleTokenAsync(string idToken)
    {
        
        var clientId = _configuration["Authentication:Google:ClientId"];
        var payload = await GoogleJsonWebSignature
            .ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { clientId }
        });

        if (!payload.EmailVerified)
            throw new WalletHubException("Google email is not verified.");


        return new GoogleUserInfo
        {
            Id = payload.Subject,
            Email = payload.Email,
            Name = payload.Name,
            Picture = payload.Picture,
            EmailVerified = payload.EmailVerified
        };
        
    }

    private async Task<string> GenerateUniqueUsernameAsync(string email)
    {
        var baseUsername = email.Split('@')[0];
        var username = baseUsername;
        var suffix = 0;

        while (await _userManager.FindByNameAsync(username) is not null)
        {
            suffix = new Random().Next(1000, 9999);
            username = $"{baseUsername}{suffix}";
        }

        return username;
    }

    private async Task SaveRefreshTokenAsync(AppUser user, string refreshToken)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);
    }
}