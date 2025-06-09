using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;

namespace WalletHub.API.Caching
{
    public class CachedPortfolioService : IPortfolioService
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ICacheService _cache;

        public CachedPortfolioService(IPortfolioService portfolioService, ICacheService cache)
        {
            _portfolioService = portfolioService;
            _cache = cache;
        }

        public async Task<List<Asset>> GetUserAssets(AppUser user, int portfolioId, CancellationToken cancellationToken)
        {
            return await _portfolioService.GetUserAssets(user, portfolioId, cancellationToken);
        }

        public async Task<PortfolioDto> CreatePortfolio(AppUser user, CreatePortfolioDto dto, CancellationToken cancellationToken)
        {
            var portfolio = await _portfolioService.CreatePortfolio(user, dto, cancellationToken);

            var cacheKey = $"User_{user.Id}_Portfolios";

            var cachedPortfolios = await _cache.GetAsync<List<PortfolioDto>>(cacheKey);

            if (cachedPortfolios != null)
            {
                cachedPortfolios.Add(portfolio);
                
                await _cache.SetAsync(cacheKey, cachedPortfolios, TimeSpan.FromMinutes(5));
            }
            else
            {
                await _cache.RemoveAsync(cacheKey);
            }

            return portfolio;
        }


        public async Task<PortfolioDto> GetPortfolioById(int portfolioId, CancellationToken cancellationToken)
        {
            return await _portfolioService.GetPortfolioById(portfolioId, cancellationToken);
        }

        public async Task<PortfolioTotalValue> GetPortfolioTotalValue(int portfolioId, CancellationToken cancellationToken)
        {
            var cacheKey = $"Portfolio_{portfolioId}_PortfolioTotalValue"; 
            var cachedValues = await _cache.GetAsync<PortfolioTotalValue>(cacheKey);

            if (cachedValues != null)
            {
                return cachedValues;
            }

            var portfolioValue = await _portfolioService.GetPortfolioTotalValue(portfolioId, cancellationToken);

            await _cache.SetAsync(cacheKey, portfolioValue, TimeSpan.FromMinutes(5));

            return portfolioValue;
        }

        public async Task<PortfolioDailyChange> GetPortfolioDailyChange(int portfolioId, CancellationToken cancellationToken)
        {  
            var cacheKey = $"Portfolio_{portfolioId}_PortfolioDailyChange";
            var cachedChange = await _cache.GetAsync<PortfolioDailyChange>(cacheKey);

            if (cachedChange != null)
            {
                return cachedChange;
            }

            var dailyChange = await _portfolioService.GetPortfolioDailyChange(portfolioId, cancellationToken);

            await _cache.SetAsync(cacheKey, dailyChange, TimeSpan.FromMinutes(5));

            return dailyChange;
            
            
        }

        public async Task<PortfolioDto> DeleteAsync(AppUser user, int portfolioId, CancellationToken cancellationToken)
        {
            
            var portfolio = await _portfolioService.DeleteAsync(user, portfolioId, cancellationToken);

            var portfolioCacheKey = $"Portfolio_{portfolioId}_PortfolioTotalValue";
            await _cache.RemoveAsync(portfolioCacheKey);

            var portfoliosCacheKey = $"User_{user.Id}_Portfolios";
            var cachedPortfolios = await _cache.GetAsync<List<PortfolioDto>>(portfoliosCacheKey);

            if (cachedPortfolios != null)
            {
                var portfolioToRemove = cachedPortfolios.FirstOrDefault(p => p.Id == portfolioId);
                if (portfolioToRemove != null)
                {
                    cachedPortfolios.Remove(portfolioToRemove);

                    await _cache.SetAsync(portfoliosCacheKey, cachedPortfolios, TimeSpan.FromMinutes(5));
                }
            }

            return portfolio;
        }

        public async Task<List<PortfolioDto>> GetAllUserPortfolios(string userId, CancellationToken cancellationToken)
        {
            var cacheKey = $"User_{userId}_Portfolios";
            var cachedData = await _cache.GetAsync<List<PortfolioDto>>(cacheKey);

            if (cachedData != null)
            {
                return cachedData;
            }

            var portfolioDtos = await _portfolioService.GetAllUserPortfolios(userId, cancellationToken);

            await _cache.SetAsync(cacheKey, portfolioDtos, TimeSpan.FromMinutes(5));

            return portfolioDtos;
        }

        public async Task<PortfolioDto> UpdateNameAsync(AppUser user, int portfolioId, string newPortfolioName, CancellationToken cancellationToken)
        {
            var updatedPortfolio = await _portfolioService.UpdateNameAsync(user, portfolioId, newPortfolioName, cancellationToken);

            var cacheKey = $"User_{user.Id}_Portfolios";
            var cachedPortfolios = await _cache.GetAsync<List<PortfolioDto>>(cacheKey);

            if (cachedPortfolios != null)
            {
                var index = cachedPortfolios.FindIndex(p => p.Id == updatedPortfolio.Id);
                if (index != -1)
                {
                    cachedPortfolios[index].Name = updatedPortfolio.Name;
                    await _cache.SetAsync(cacheKey, cachedPortfolios, TimeSpan.FromMinutes(5));
                }
            }

            return updatedPortfolio;
        }

        public Task<List<PortfolioDto>> GetAllPortfoliosAsync(CancellationToken cancellationToken)
        {
            var portfolios = _portfolioService.GetAllPortfoliosAsync(cancellationToken);
            return portfolios;
        }
    }
}
