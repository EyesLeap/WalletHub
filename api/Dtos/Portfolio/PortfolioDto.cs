using System.ComponentModel.DataAnnotations;
using api.Dtos.AssetDtos;
using api.Dtos.TransactionDtos;
using api.Models;

namespace api.Dtos.Portfolio
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