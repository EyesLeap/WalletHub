using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WalletHub.API.Dtos.Comment;

namespace WalletHub.API.Dtos.AssetDtos
{
    public class AddAssetDto
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Symbol cannot be longer than 10 characters")]
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Purchase price must be greater than zero")]
        public decimal PurchasePrice { get; set; }
        
        
    }

}