using api.Dtos.AssetDtos;
using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
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