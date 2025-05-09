using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Exceptions;
using WalletHub.API.Extensions;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WalletHub.API.Controllers
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
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedException("User is not authorized.");

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                throw new UserNotFoundException("User not found.");

            var currencies = await _assetService.GetAllByPortfolioIdAsync(portfolioId);
            return Ok(currencies);
        }


        [HttpDelete("{symbol}")]
        public async Task<IActionResult> RemoveAsset(int portfolioId, string symbol)
        {
            var username = User.GetUsername();
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedException("User is not authorized.");

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                throw new UserNotFoundException("User not found.");

            var success = await _assetService.RemoveAssetAsync(portfolioId, symbol);
            if (!success)
                throw new AssetNotFoundException("Currency not found in portfolio.");

            return NoContent();
        }

    }

}