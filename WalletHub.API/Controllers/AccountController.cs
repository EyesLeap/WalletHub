using WalletHub.API.Caching;
using WalletHub.API.Dtos.Account;
using WalletHub.API.Exceptions;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WalletHub.API.Dtos.AuthToken;

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

            await _accountService
                .ResendConfirmationEmailAsync(resendDto.Email, baseConfirmationLink);

            return Ok();
           
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tokenResponse = await _accountService.RefreshTokenAsync(request);
            return Ok(tokenResponse);
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid user");

            var result = await _accountService.RevokeRefreshTokenAsync(userId);
            return Ok(new { success = result, message = "Refresh token revoked successfully" });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Invalid user");

            await _accountService.RevokeRefreshTokenAsync(userId);
            return Ok(new { message = "Logged out successfully" });
        }

    }
}