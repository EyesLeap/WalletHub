using WalletHub.API.Dtos.Comment;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> GetById(int portfolioId, CancellationToken cancellationToken);
        Task<Portfolio?> GetByNameAsync(string userId, string portfolioName, CancellationToken cancellationToken);
        Task<List<Portfolio>> GetAllUserPortfolios(string userId, CancellationToken cancellationToken);
        Task<List<Portfolio>> GetAllPortfoliosAsync(CancellationToken cancellationToken);
        Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken cancellationToken);
        Task<Portfolio?> DeleteAsync(AppUser appUser, int portfolioId, CancellationToken cancellationToken);
        Task<Portfolio?> UpdateNameAsync(AppUser appUser, int portfolioId, string newPortfolioName, CancellationToken cancellationToken);
    }

}