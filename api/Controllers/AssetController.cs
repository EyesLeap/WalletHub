using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.TransactionDtos;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/assets/{portfolioId}")]
    [ApiController]
    [Authorize]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly UserManager<AppUser> _userManager;

        public AssetController(IAssetService assetService, UserManager<AppUser> userManager)
        {
            _assetService = assetService;
            _userManager = userManager;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAssets(int portfolioId)
        {
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var currencies = await _assetService.GetAllByPortfolioIdAsync(portfolioId);
            return Ok(currencies);
        }

        [HttpDelete("{symbol}")]
        public async Task<IActionResult> RemoveAsset(int portfolioId, string symbol)
        {
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var success = await _assetService.RemoveAssetAsync(portfolioId, symbol);
            if (!success) return NotFound("Currency not found in portfolio");

            return NoContent();
        }
    }

}