using System.ComponentModel.DataAnnotations;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Models;

namespace WalletHub.API.Dtos.Portfolio
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public string Name {get; set;}  
        public decimal TotalValueUSD { get; set; }
        public List<GetAssetDto> Assets { get; set; } = new();  
        public List<TransactionDto> Transactions { get; set; } = new();  
        public DateTime CreatedAt { get; set; }
    }

}