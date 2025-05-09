using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WalletHub.API.Dtos.Comment;

namespace WalletHub.API.Dtos.AssetDtos
{
    public class GetAssetDto
    {
        public int Id { get; set; }  
        public int PortfolioId { get; set; }  
        public string Symbol { get; set; } = string.Empty;  
        public string Name { get; set; } = string.Empty;  
        public decimal Amount { get; set; }  
        public decimal AveragePurchasePrice { get; set; }  
    }

}