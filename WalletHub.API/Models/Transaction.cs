using System.ComponentModel.DataAnnotations.Schema;
using WalletHub.API.Helpers;

namespace WalletHub.API.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int PortfolioId { get; set; }
        public Portfolio Portfolio { get; set; }
        public int AssetId { get; set; }
        public Asset Asset { get; set; }
        [Column(TypeName = "decimal(34,18)")] 
        public decimal Amount { get; set; } 
        [Column(TypeName = "decimal(34,18)")]
        public decimal PricePerCoin { get; set; } 
        [Column(TypeName = "decimal(34,18)")]
        public decimal TotalCost { get; set; }  
        public TransactionType TransactionType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}