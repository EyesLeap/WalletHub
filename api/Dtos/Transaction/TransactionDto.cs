using api.Helpers;

namespace api.Dtos.TransactionDtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;  
        public decimal Amount { get; set; }  
        public decimal PricePerCoin { get; set; }  
        public decimal TotalCost { get; set; }  
        public TransactionType TransactionType { get; set; } 
        public DateTime CreatedAt { get; set; }  
    }

}