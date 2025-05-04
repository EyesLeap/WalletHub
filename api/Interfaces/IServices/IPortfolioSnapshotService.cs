using api.Dtos.AssetDtos;
using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Helpers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    public interface IPortfolioSnapshotService
    {
        Task<PortfolioSnapshot> CreateSnapshotAsync(int portfolioId, PortfolioTotalValue portfolioTotalValue);
        Task<PortfolioSnapshot> GetLatestDailySnapshot(int portfolioId);
        Task<IEnumerable<PortfolioSnapshot>> GetPortfolioSnapshotsAsync(int portfolioId, PortfolioSnapshotRange range);
    }

}