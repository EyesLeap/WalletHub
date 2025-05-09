using System.ComponentModel.DataAnnotations;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;

namespace WalletHub.API.Dtos.Portfolio
{
    public class PortfolioDailyChange
    {
        public decimal PercentChange24h { get; set; }  
        public decimal ProfitLoss {get; set;}
    }

}