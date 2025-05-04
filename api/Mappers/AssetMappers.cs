using api.Dtos.Currency;
using api.Dtos.AssetDtos;
using api.Models;

namespace api.Mappers
{
    public static class AssetMappers
    {
        public static GetAssetDto ToGetAssetDto(this Asset asset)
        {
            return new GetAssetDto
            {
                Id = asset.Id,
                PortfolioId = asset.PortfolioId,
                Symbol = asset.Symbol, 
                Name = asset.Name,     
                Amount = asset.Amount,
                AveragePurchasePrice = asset.AveragePurchasePrice
            };
        }
    }
}