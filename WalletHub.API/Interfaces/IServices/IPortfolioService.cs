using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace WalletHub.API.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioDto> CreatePortfolio(AppUser user, CreatePortfolioDto dto);
        Task<List<Asset>> GetUserAssets(AppUser user, int portfolioId);
        Task<List<PortfolioDto>> GetAllUserPortfolios(string userId);
        Task<Portfolio> GetPortfolioById(int portfolioId);
        Task<List<Portfolio>> GetAllPortfoliosAsync();
        Task<PortfolioTotalValue> GetPortfolioTotalValue(int portfolioId);
        Task<PortfolioDailyChange> GetPortfolioDailyChange(int portfolioId);
        Task<PortfolioDto> UpdateNameAsync(AppUser user, int portfolioId, string newPortfolioName);
        Task<Portfolio> DeleteAsync(AppUser user, int portfolioId);

    }
}