using System.ComponentModel.DataAnnotations;
using api.Dtos.AssetDtos;
using api.Models;

namespace api.Dtos.Portfolio
{
    public class PortfolioTotalValue
    {
        public decimal TotalValueUSD { get; set; }
        public List<AssetValue> AssetValues { get; set; } = new();
    }

}