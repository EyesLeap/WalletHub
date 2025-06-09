using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace WalletHub.API.Interfaces
{
    public interface IPortfolioService
    {
        Task<PortfolioDto> CreatePortfolio(AppUser user, CreatePortfolioDto dto, CancellationToken cancellationToken);
        Task<List<Asset>> GetUserAssets(AppUser user, int portfolioId, CancellationToken cancellationToken);
        Task<List<PortfolioDto>> GetAllUserPortfolios(string userId, CancellationToken cancellationToken);
        Task<PortfolioDto> GetPortfolioById(int portfolioId, CancellationToken cancellationToken);
        Task<List<PortfolioDto>> GetAllPortfoliosAsync(CancellationToken cancellationToken);
        Task<PortfolioTotalValue> GetPortfolioTotalValue(int portfolioId, CancellationToken cancellationToken);
        Task<PortfolioDailyChange> GetPortfolioDailyChange(int portfolioId, CancellationToken cancellationToken);
        Task<PortfolioDto> UpdateNameAsync(AppUser user, int portfolioId, string newPortfolioName, CancellationToken cancellationToken);
        Task<PortfolioDto> DeleteAsync(AppUser user, int portfolioId, CancellationToken cancellationToken);
    }

}