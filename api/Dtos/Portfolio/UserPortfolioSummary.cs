using System.ComponentModel.DataAnnotations;
using api.Dtos.AssetDtos;
using api.Models;

namespace api.Dtos.Portfolio
{
    public class UserPortfolioSummary
    {
        public decimal TotalValueUSD { get; set; }
        
    }

}