using api.Dtos.Comment;
using api.Models;

namespace api.Interfaces
{
    public interface IPortfolioRepository
    {
       Task<Portfolio> GetPortfolioById(int portfolioId);
       Task<List<Portfolio>> GetAllUserPortfolios(string userId);
       Task<List<Portfolio>> GetAllPortfoliosAsync();
       Task<Portfolio> AddAsync(Portfolio portfolio);
       Task<Portfolio> DeleteAsync(AppUser appUser, int portfolioId);
       Task<Portfolio> UpdateNameAsync(AppUser appUser, int portfolioId, string newPortfolioName);
        
    }
}