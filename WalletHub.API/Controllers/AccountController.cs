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

        public AccountController(
         IAccountService accountService)
        {
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

            var baseConfirmationLink = Url.Action(
                nameof(ConfirmEmail),
                "Account",
                null,
                Request.Scheme);

            var result = await _accountService.RegisterWithConfirmationAsync(registerDto, baseConfirmationLink);

            return Ok(result);


        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _accountService.ConfirmEmailAsync(userId, token);
            return Ok(result);

        }
        
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationDto resendDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
            var baseConfirmationLink = Url.Action(
                nameof(ConfirmEmail), 
                "Account",
                null,
                Request.Scheme);

            var result = await _accountService.ResendConfirmationEmailAsync(resendDto.Email, baseConfirmationLink);
            return Ok(new { message = result });
           
        }

    }
}