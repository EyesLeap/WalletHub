using WalletHub.API.Dtos.Comment;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface IAssetRepository
    {
        Task<Asset> AddAsync(Asset portfolio);
        Task<Asset> GetByPortfolioAndSymbol(int portfolioId, string symbol);
        Task<IEnumerable<GetAssetDto>> GetAllByPortfolioIdAsync(int portfolioId);
        Task<Asset?> UpdateAsync(Asset asset);
        Task<Asset?> DeleteAsync(int portfolioId, string symbol);
    }
}