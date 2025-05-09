using WalletHub.API.Data;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace WalletHub.API.Repository
{
    public class PortfolioSnapshotRepository : IPortfolioSnapshotRepository
    {
        private readonly ApplicationDBContext _context;

        public PortfolioSnapshotRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<PortfolioSnapshot> AddSnapshotAsync(PortfolioSnapshot snapshot)
        {
            await _context.PortfolioSnapshots.AddAsync(snapshot);
            await _context.SaveChangesAsync();
            return snapshot;
        }

        public IQueryable<PortfolioSnapshot> GetPortfolioSnapshotsQueryable(int portfolioId)
        {
            return _context.PortfolioSnapshots
                .Where(s => s.PortfolioId == portfolioId);
        }

        public async Task<PortfolioSnapshot?> GetLatestSnapshotAsync(int portfolioId)
        {
            return await _context.PortfolioSnapshots
                .Where(s => s.PortfolioId == portfolioId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}