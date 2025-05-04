using api.Dtos.Comment;
using api.Dtos.AssetDtos;
using api.Models;

namespace api.Interfaces
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