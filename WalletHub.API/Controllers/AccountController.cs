using WalletHub.API.Caching;
using WalletHub.API.Dtos.Account;
using WalletHub.API.Exceptions;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WalletHub.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSenderService _emailSenderService;
        public AccountController(UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager,
         ITokenService tokenService,
         IEmailSenderService emailSenderService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailSenderService = emailSenderService;

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

            if (user is null)
                throw new UnauthorizedException();

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                throw new UnauthorizedException();

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!createdUser.Succeeded)
                throw new UserNotFoundException(appUser.UserName);

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
            if (!roleResult.Succeeded)
                throw new WalletHubException("Failed to assign role");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                new { userId = appUser.Id, token }, Request.Scheme);

            var subject = "Confirm your email - WalletHub";
            var message = $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";

            await _emailSenderService.SendEmailAsync(appUser.Email, subject, message);

            return Ok("Registration successful. Please check your email to confirm your account.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId is null || token is null)
                return BadRequest("Invalid email confirmation request.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound($"User with ID {userId} not found.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return Ok("Email confirmed successfully. You can now log in.");
            else
                return BadRequest("Email confirmation failed.");
        }

    }
}