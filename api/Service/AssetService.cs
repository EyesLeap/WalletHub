using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Dtos.AssetDtos;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Identity;

namespace api.Service
{
    public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IPortfolioRepository _portfolioRepository;

    public AssetService(IAssetRepository assetRepository, IPortfolioRepository portfolioRepository)
    {
        _assetRepository = assetRepository;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<IEnumerable<GetAssetDto>> GetAllByPortfolioIdAsync(int portfolioId)
    {
        var portfolio = await _portfolioRepository.GetPortfolioById(portfolioId);
        if (portfolio == null) return Enumerable.Empty<GetAssetDto>();

        return await _assetRepository.GetAllByPortfolioIdAsync(portfolioId);
    }

    public async Task<bool> RemoveAssetAsync(int portfolioId, string symbol)
    {
        await _assetRepository.DeleteAsync(portfolioId, symbol);
        return true;
    }
}

}