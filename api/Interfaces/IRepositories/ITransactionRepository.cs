using api.Dtos.Currency;
using api.Helpers;
using api.Models;

namespace api.Interfaces
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