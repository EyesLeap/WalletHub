using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Repository;
using Microsoft.AspNetCore.Identity;

namespace WalletHub.API.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletHub.API.Dtos.Account;
using WalletHub.API.Exceptions;

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

    public async Task<AppUser> RegisterAsync(RegisterDto registerDto)
    {
        var appUser = new AppUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email
        };

        var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
        if (!createdUser.Succeeded)
            return null;

        var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
        if (!roleResult.Succeeded)
            throw new WalletHubException("Failed to assign role");

        return appUser;
    }

    public async Task<NewUserDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

        if (user is null)
            throw new UnauthorizedException();

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
            throw new UnauthorizedException();

        return new NewUserDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = _tokenService.CreateToken(user)
        };
    }

    public async Task<string> SendConfirmationEmailAsync(AppUser appUser, string confirmationLink)
    {   
        var subject = "Confirm your email - WalletHub";

        await _emailSenderService.SendEmailAsync(appUser.Email, subject, appUser.UserName, confirmationLink);

        return "Registration successful. Please check your email to confirm your account.";
    }

    public async Task<string> ConfirmEmailAsync(string userId, string token)
    {
        if (userId is null || token is null)
            throw new ArgumentException("Invalid email confirmation request.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException($"User with ID {userId} not found.");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return "Email confirmed successfully. You can now log in.";
        else
            throw new WalletHubException("Email confirmation failed.");
    }


}