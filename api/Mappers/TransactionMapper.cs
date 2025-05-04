using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.TransactionDtos;
using api.Models;

namespace api.Mappers
{
    public static class TransactionMapper
    {
       public static TransactionDto ToTransactionDto(this Transaction transaction)
        {
            return new TransactionDto
            {          
                Id = transaction.Id,
                Symbol = transaction.Asset.Symbol,
                Amount = transaction.Amount,
                PricePerCoin = transaction.PricePerCoin,
                TotalCost = transaction.TotalCost,
                TransactionType = transaction.TransactionType,
                CreatedAt = transaction.CreatedAt
            };
        }

        
    }
}