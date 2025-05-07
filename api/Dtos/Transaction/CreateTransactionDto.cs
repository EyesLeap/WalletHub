using System.ComponentModel.DataAnnotations;
using System.Transactions;
using api.Helpers;

namespace api.Dtos.TransactionDtos
{
    public class CreateTransactionDto
    {
        public string Symbol { get; set; } = string.Empty;
  
        public decimal Amount { get; set; }
  
        public decimal PricePerCoin { get; set; }
        
        public TransactionType TransactionType { get; set; }  

        public DateTime CreatedAt { get; set; }  
    }

}