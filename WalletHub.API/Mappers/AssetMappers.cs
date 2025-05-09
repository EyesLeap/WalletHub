using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;

namespace WalletHub.API.Mappers
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