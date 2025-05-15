using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Repository;
using Microsoft.AspNetCore.Identity;

namespace WalletHub.API.Service
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
            var portfolio = await _portfolioRepository.GetById(portfolioId);
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