using WalletHub.API.Data;
using WalletHub.API.Exceptions;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace WalletHub.API.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> AddAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio> DeleteAsync(AppUser appUser, int portfolioId)
        {
            var portfolioModel = await _context.Portfolios.FirstOrDefaultAsync(p => p.AppUser.Id == appUser.Id && 
            p.Id == portfolioId);

            if (portfolioModel == null)
            {
                return null;
            }

            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync();
            return portfolioModel;

            
        }

        public Task<List<Portfolio>> GetAllPortfoliosAsync()
        {
             return _context.Portfolios
                .Include(p => p.Transactions)
                .Include(p => p.Assets)
                .ToListAsync();
        }


        public async Task<List<Portfolio>> GetAllUserPortfolios(string userId)
        {
            return await _context.Portfolios
                .Include(p => p.Assets) 
                .Include(p => p.Transactions)  
                .Where(p => p.AppUserId == userId)
                .ToListAsync();
        }   


        public async Task<Portfolio> GetById(int portfolioId)
        {
            var portfolioModel = await _context.Portfolios
                .Include(p => p.Assets) 
                .Include(p => p.Transactions)  
                .FirstOrDefaultAsync(p => p.Id == portfolioId);

            if (portfolioModel == null)
            {
                throw new NotFoundException($"Portfolio with ID {portfolioId} not found.");
            }

            return portfolioModel;
        }

    public async Task<Portfolio?> GetByNameAsync(string userId, string portfolioName)
    {
       var portfolioModel = await _context.Portfolios
                .Include(p => p.Assets) 
                .Include(p => p.Transactions)  
                .FirstOrDefaultAsync(p => p.Name == portfolioName && p.AppUserId == userId);

            return portfolioModel;
    }

    public async Task<Portfolio> UpdateNameAsync(AppUser appUser, int portfolioId, string newPortfolioName)
        {
            var portfolioModel = await _context.Portfolios
                .FirstOrDefaultAsync(p => p.AppUserId == appUser.Id && 
                p.Id == portfolioId);
        
            if (portfolioModel == null) return null;
                
            portfolioModel.Name = newPortfolioName;
            await _context.SaveChangesAsync();

            return portfolioModel;
        }
    }
}