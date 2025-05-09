using System.ComponentModel.DataAnnotations;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;

namespace WalletHub.API.Dtos.Portfolio
{
    public class PortfolioTotalValue
    {
        public decimal TotalValueUSD { get; set; }
        public List<AssetValue> AssetValues { get; set; } = new();
    }

}