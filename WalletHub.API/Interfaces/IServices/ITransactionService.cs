using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace WalletHub.API.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction?> CreateTransactionAsync(int portfolioId, CreateTransactionDto dto);
        Task<Transaction> GetTransactionByIdAsync(int transactionId);
        Task<PagedList<TransactionDto>> GetAllTransactionsAsync(int portfolioId, TransactionQueryDto queryDto);
        Task<Transaction?> DeleteAsync(int transactionId);

    }
}