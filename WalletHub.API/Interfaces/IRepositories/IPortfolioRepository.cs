using WalletHub.API.Dtos.Comment;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface IPortfolioRepository
    {
       Task<Portfolio> GetById(int portfolioId);
       Task<Portfolio?> GetByNameAsync(string userId, string portfolioName);
       Task<List<Portfolio>> GetAllUserPortfolios(string userId);
       Task<List<Portfolio>> GetAllPortfoliosAsync();
       Task<Portfolio> AddAsync(Portfolio portfolio);
       Task<Portfolio> DeleteAsync(AppUser appUser, int portfolioId);
       Task<Portfolio> UpdateNameAsync(AppUser appUser, int portfolioId, string newPortfolioName);
        
    }
}