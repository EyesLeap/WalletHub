using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletHub.API.Dtos.Account;
using WalletHub.API.Dtos.AuthToken;
using WalletHub.API.Exceptions;
using WalletHub.API.Exceptions.NotFound;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;

namespace WalletHub.API.Service;

public class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailSenderService _emailSenderService;

    public AccountService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IEmailSenderService emailSenderService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailSenderService = emailSenderService;
    }

    public async Task<AuthResponseDto> RegisterWithConfirmationAsync(RegisterDto registerDto, string confirmationLink)
    {
        var appUser = await RegisterAsync(registerDto);
        if (appUser is null)
            throw new WalletHubException("User registration failed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        var finalConfirmationLink = $"{confirmationLink}?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";

        await SendConfirmationEmailAsync(appUser, finalConfirmationLink);

        return new AuthResponseDto
        {
            UserName = appUser.UserName,
            Email = appUser.Email,
            TokenResponse = await _tokenService.CreateTokenResponse(appUser)
        };
    }

    public async Task<AppUser> RegisterAsync(RegisterDto registerDto)
    {
        var existingUserByName = await _userManager.FindByNameAsync(registerDto.UserName);
        if (existingUserByName is not null)
            throw new WalletHubException("User with this username already exists.");

        var appUser = new AppUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            EmailConfirmed = false 
        };

        var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
        if (!createdUser.Succeeded)
        {
            var errors = string.Join(", ", createdUser.Errors.Select(e => e.Description));
            throw new WalletHubException($"User creation failed: {errors}");
        }

        var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(appUser);
            throw new WalletHubException("Failed to assign role to user.");
        }

        return appUser;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

        if (user is null)
            throw new UnauthorizedException("Invalid username or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid username or password.");

        var tokenResponse = await _tokenService.CreateTokenResponse(user);

        await SaveRefreshTokenAsync(user, tokenResponse.RefreshToken);

        return new AuthResponseDto
        {
            UserName = user.UserName,
            Email = user.Email,
            TokenResponse = tokenResponse
        };
    }

    public async Task SendConfirmationEmailAsync(AppUser appUser, string confirmationLink)
    {   
        if (appUser is null)
            throw new ArgumentNullException(nameof(appUser));

        if (string.IsNullOrEmpty(confirmationLink))
            throw new ArgumentException("Confirmation link cannot be null or empty.", nameof(confirmationLink));

        var subject = "Confirm your email - WalletHub";
  
        await _emailSenderService.SendEmailAsync(appUser.Email, subject, appUser.UserName, confirmationLink);            
    }

    public async Task ResendConfirmationEmailAsync(string email, string confirmationLink)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            throw new UserNotFoundException(email);

        if (user.EmailConfirmed)
            throw new WalletHubException("Email is already confirmed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var finalConfirmationLink = $"{confirmationLink}?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        await SendConfirmationEmailAsync(user, finalConfirmationLink);
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            throw new ArgumentException("Invalid email confirmation request.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException(userId);

        if (user.EmailConfirmed)
            throw new WalletHubException("Email is already confirmed.");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
            return true;
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new WalletHubException($"Email confirmation failed: {errors}");
        }
    }

    public async Task<AuthTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {   
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Invalid token");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null || user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }

        var tokenResponse = await _tokenService.CreateTokenResponse(user);

        user.RefreshToken = tokenResponse.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return tokenResponse;
       
    }

    public async Task<bool> RevokeRefreshTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException(userId);

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new WalletHubException("Failed to revoke refresh token");

        return true;
    }

    private async Task SaveRefreshTokenAsync(AppUser user, string refreshToken)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);
    }
}