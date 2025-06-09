using WalletHub.API.Data;
using WalletHub.API.Exceptions;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WalletHub.API.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken cancellationToken = default)
        {
            await _context.Portfolios.AddAsync(portfolio, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return portfolio;
        }

        public async Task<Portfolio?> DeleteAsync(AppUser appUser, int portfolioId, CancellationToken cancellationToken = default)
        {
            var portfolioModel = await _context.Portfolios.FirstOrDefaultAsync(
                p => p.AppUser.Id == appUser.Id && p.Id == portfolioId, cancellationToken);

            if (portfolioModel is null)           
                return null;       

            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync(cancellationToken);
            return portfolioModel;
        }

        public Task<List<Portfolio>> GetAllPortfoliosAsync(CancellationToken cancellationToken = default)
        {
            return _context.Portfolios
                .Include(p => p.Transactions)
                .Include(p => p.Assets)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Portfolio>> GetAllUserPortfolios(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Portfolios
                .Include(p => p.Assets)
                .Include(p => p.Transactions)
                .Where(p => p.AppUserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Portfolio?> GetById(int portfolioId, CancellationToken cancellationToken = default)
        {
            var portfolioModel = await _context.Portfolios
                .Include(p => p.Assets)
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Id == portfolioId, cancellationToken);

            return portfolioModel;
        }

        public async Task<Portfolio?> GetByNameAsync(string userId, string portfolioName, CancellationToken cancellationToken = default)
        {
            var portfolioModel = await _context.Portfolios
                .Include(p => p.Assets)
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Name == portfolioName && p.AppUserId == userId, cancellationToken);

            return portfolioModel;
        }

        public async Task<Portfolio?> UpdateNameAsync(AppUser appUser, int portfolioId, string newPortfolioName, CancellationToken cancellationToken = default)
        {
            var portfolioModel = await _context.Portfolios
                .FirstOrDefaultAsync(p => p.AppUserId == appUser.Id && p.Id == portfolioId, cancellationToken);

            if (portfolioModel is null)
                return null;

            portfolioModel.Name = newPortfolioName;
            await _context.SaveChangesAsync(cancellationToken);

            return portfolioModel;
        }
    }
}
