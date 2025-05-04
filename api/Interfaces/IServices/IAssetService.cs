using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.AssetDtos;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    public interface IAssetService
    {
        Task<IEnumerable<GetAssetDto>> GetAllByPortfolioIdAsync(int portfolioId);
        Task<bool> RemoveAssetAsync(int portfolioId, string symbol);

    }
}