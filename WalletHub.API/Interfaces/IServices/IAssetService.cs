using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace WalletHub.API.Interfaces
{
    public interface IAssetService
    {
        Task<IEnumerable<GetAssetDto>> GetAllByPortfolioIdAsync(int portfolioId);
        Task<bool> RemoveAssetAsync(int portfolioId, string symbol);

    }
}