using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Helpers;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace WalletHub.API.Interfaces
{
    public interface IPortfolioSnapshotService
    {
        Task<PortfolioSnapshot> CreateSnapshotAsync(int portfolioId, PortfolioTotalValue portfolioTotalValue);
        Task<PortfolioSnapshot> GetLatestDailySnapshot(int portfolioId);
        Task<IEnumerable<PortfolioSnapshot>> GetPortfolioSnapshotsAsync(int portfolioId, PortfolioSnapshotRange range);
    }

}