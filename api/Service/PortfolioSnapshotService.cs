using api.Dtos.AssetDtos;
using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace api.Service
{
    public class PortfolioSnapshotService : IPortfolioSnapshotService
    {
        private readonly IPortfolioSnapshotRepository _snapshotRepository;
        
        public PortfolioSnapshotService(IPortfolioSnapshotRepository snapshotRepository)
        {
            _snapshotRepository = snapshotRepository;
        }
    
        public async Task<PortfolioSnapshot> CreateSnapshotAsync(int portfolioId, PortfolioTotalValue portfolioTotalValue)
        {       
            var snapshot = new PortfolioSnapshot
            {
                PortfolioId = portfolioId,
                TotalValueUSD = portfolioTotalValue.TotalValueUSD,
                CreatedAt = DateTime.UtcNow
            };

            await _snapshotRepository.AddSnapshotAsync(snapshot);
            return snapshot;
        }
        
        public async Task<PortfolioSnapshot> GetLatestDailySnapshot(int portfolioId)
        {
            var today = DateTime.UtcNow.Date;
            var snapshots = _snapshotRepository.GetPortfolioSnapshotsQueryable(portfolioId);

            return await snapshots
                .Where(s => s.CreatedAt < today) 
                .OrderByDescending(s => s.CreatedAt) 
                .FirstOrDefaultAsync();
        }
        
        public async Task<IEnumerable<PortfolioSnapshot>> GetPortfolioSnapshotsAsync(int portfolioId, PortfolioSnapshotRange range)
        {
            var snapshots = _snapshotRepository.GetPortfolioSnapshotsQueryable(portfolioId);
            var now = DateTime.UtcNow;

            switch (range)
            {
                case PortfolioSnapshotRange.Last24Hours:
                    snapshots = snapshots.Where(s => s.CreatedAt >= now.AddHours(-24));
                    break;
                case PortfolioSnapshotRange.Last7Days:
                    snapshots = snapshots.Where(s => s.CreatedAt >= now.AddDays(-7));
                    break;
                case PortfolioSnapshotRange.Last30Days:
                    snapshots = snapshots.Where(s => s.CreatedAt >= now.AddDays(-30));
                    break;
                default:
                    break;
            }

            return await snapshots
                .OrderBy(s => s.CreatedAt)
                .ToListAsync(); 
        }


    }

}