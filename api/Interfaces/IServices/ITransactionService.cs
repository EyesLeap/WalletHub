using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.TransactionDtos;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction?> CreateTransactionAsync(int portfolioId, CreateTransactionDto dto);
        Task<Transaction> GetTransactionByIdAsync(int transactionId);
        Task<PagedList<TransactionDto>> GetAllTransactionsAsync(int portfolioId, TransactionQueryDto queryDto);
        Task<Transaction?> DeleteAsync(int transactionId);

    }
}