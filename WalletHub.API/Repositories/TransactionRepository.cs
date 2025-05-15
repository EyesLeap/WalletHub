using WalletHub.API.Data;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using Microsoft.EntityFrameworkCore;

namespace WalletHub.API.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDBContext _context;

        public TransactionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Portfolio)
                .Include(t => t.Asset)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public IQueryable<Transaction> GetAllByPortfolioId(int portfolioId)
        {
             return _context.Transactions
                .Include(t => t.Portfolio)
                .Include(t => t.Asset)
                .Where(t => t.PortfolioId == portfolioId)
                .OrderByDescending(t => t.CreatedAt);
        }

        public async Task<Transaction> AddAsync(Transaction transactionModel)
        {
            await _context.Transactions.AddAsync(transactionModel);
            await _context.SaveChangesAsync();
            return transactionModel;
        }

        public async Task<Transaction?> UpdateAsync(Transaction transactionModel)
        {
            _context.Transactions.Update(transactionModel);
            await _context.SaveChangesAsync();
            return transactionModel;
        }

        public async Task<Transaction?> DeleteAsync(int id)
        {
            var transactionModel = await _context.Transactions.FindAsync(id);

            if (transactionModel == null) return null;

            _context.Transactions.Remove(transactionModel);
            await _context.SaveChangesAsync(); 
            return transactionModel;
        }
    }
}