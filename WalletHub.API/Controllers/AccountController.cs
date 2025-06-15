using WalletHub.API.Caching;
using WalletHub.API.Dtos.Account;
using WalletHub.API.Exceptions;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace WalletHub.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSenderService _emailSenderService;
        public AccountController(UserManager<AppUser> userManager,
         SignInManager<AppUser> signInManager,
         ITokenService tokenService,
         IEmailSenderService emailSenderService,
         IAccountService accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailSenderService = emailSenderService;
            _accountService = accountService;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.LoginAsync(loginDto);
            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = await _accountService.RegisterAsync(registerDto);
            if (appUser == null)
                return BadRequest("User registration failed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            var confirmationLink = Url.Action(
                nameof(ConfirmEmail), "Account",
                new { userId = appUser.Id, token = token },
                Request.Scheme);

            var result = await _accountService.SendConfirmationEmailAsync(appUser, confirmationLink);
            return Ok(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {          
            var result = await _accountService.ConfirmEmailAsync(userId, token);        
            return Ok(result);           
             
        }

    }
}