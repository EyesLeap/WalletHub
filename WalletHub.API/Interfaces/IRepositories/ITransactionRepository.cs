using WalletHub.API.Dtos.Currency;
using WalletHub.API.Helpers;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface ITransactionRepository
    {
       
        Task<Transaction?> GetByIdAsync(int id);
        IQueryable<Transaction> GetAllByPortfolioId(int portfolioId);
        Task<Transaction> AddAsync(Transaction transaction);
        Task<Transaction?> UpdateAsync(Transaction transaction);
        Task<Transaction?> DeleteAsync(int id);
    }
}