using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Export;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Helpers;
using WalletHub.API.Models;

namespace WalletHub.API.Mappers
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
        public static TransactionReportDto ToReportDto(
        this IEnumerable<TransactionDto> transactions,
        int portfolioId,
        ExportFormat format)
        {
            return new TransactionReportDto
            {
                PortfolioId = portfolioId,
                Format = format,
                Transactions = transactions.ToList()
            };
        }


        
    }
}