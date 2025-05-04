using System.ComponentModel.DataAnnotations;
using System.Transactions;
using api.Helpers;

namespace api.Dtos.TransactionDtos
{
    public class CreateTransactionDto
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters")]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal PricePerCoin { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }  

        [Required]
        public DateTime CreatedAt { get; set; }  
    }

}