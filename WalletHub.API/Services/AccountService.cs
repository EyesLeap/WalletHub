using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletHub.API.Dtos.Account;
using WalletHub.API.Exceptions;

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
        if (appUser == null)
            throw new WalletHubException("User registration failed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        var finalConfirmationLink = $"{confirmationLink}?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";

        await SendConfirmationEmailAsync(appUser, finalConfirmationLink);

        return new AuthResponseDto
        {
            UserName = appUser.UserName,
            Email = appUser.Email,
            Token = _tokenService.CreateToken(appUser)
        };
    }

    public async Task<AppUser> RegisterAsync(RegisterDto registerDto)
    {
        var existingUserByName = await _userManager.FindByNameAsync(registerDto.UserName);
        if (existingUserByName != null)
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

        return new AuthResponseDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = _tokenService.CreateToken(user)
        };
    }

    public async Task<string> SendConfirmationEmailAsync(AppUser appUser, string confirmationLink)
    {   
        if (appUser == null)
            throw new ArgumentNullException(nameof(appUser));

        if (string.IsNullOrEmpty(confirmationLink))
            throw new ArgumentException("Confirmation link cannot be null or empty.", nameof(confirmationLink));

        var subject = "Confirm your email - WalletHub";

        try
        {
            await _emailSenderService.SendEmailAsync(appUser.Email, subject, appUser.UserName, confirmationLink);
            return "Registration successful. Please check your email to confirm your account.";
        }
        catch (Exception ex)
        {
            throw new WalletHubException($"Failed to send confirmation email: {ex.Message}");
        }
    }

    public async Task<string> ConfirmEmailAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            throw new ArgumentException("Invalid email confirmation request.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException(userId);

        if (user.EmailConfirmed)
            return "Email is already confirmed. You can log in.";

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return "Email confirmed successfully. You can now log in.";
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new WalletHubException($"Email confirmation failed: {errors}");
        }
    }

    public async Task<string> ResendConfirmationEmailAsync(string email, string confirmationLink)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            throw new UserNotFoundException(email);

        if (user.EmailConfirmed)
            throw new WalletHubException("Email is already confirmed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var finalConfirmationLink = $"{confirmationLink}?userId={user.Id}&token={Uri.EscapeDataString(token)}";

        return await SendConfirmationEmailAsync(user, finalConfirmationLink);
    }
}