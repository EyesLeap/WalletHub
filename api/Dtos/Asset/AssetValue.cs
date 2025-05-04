using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.Dtos.Comment;

namespace api.Dtos.AssetDtos
{
    public class AssetValue
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePurchasePrice { get; set; }
        public decimal PercentChange1h { get; set; } 
        public decimal PercentChange24h { get; set; } 
        public decimal PercentChange7d { get; set; }  
        public decimal ProfitLoss {get; set;}
        
        
    }

}