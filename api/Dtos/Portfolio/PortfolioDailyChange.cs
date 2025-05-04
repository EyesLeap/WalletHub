using System.ComponentModel.DataAnnotations;
using api.Dtos.AssetDtos;
using api.Models;

namespace api.Dtos.Portfolio
{
    public class PortfolioDailyChange
    {
        public decimal PercentChange24h { get; set; }  
        public decimal ProfitLoss {get; set;}
    }

}