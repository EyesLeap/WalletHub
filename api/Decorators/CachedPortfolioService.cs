using api.Dtos.AssetDtos;
using api.Dtos.Currency;
using api.Dtos.Portfolio;
using api.Interfaces;
using api.Models;

namespace api.Caching
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

        public async Task<List<Asset>> GetUserAssets(AppUser user, int portfolioId)
        {
            return await _portfolioService.GetUserAssets(user, portfolioId);
        }

        public async Task<PortfolioDto> CreatePortfolio(AppUser user, CreatePortfolioDto dto)
        {
            var portfolio = await _portfolioService.CreatePortfolio(user, dto);

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


        public async Task<Portfolio> GetPortfolioById(int portfolioId)
        {
            return await _portfolioService.GetPortfolioById(portfolioId);
        }

        public async Task<PortfolioTotalValue> GetPortfolioTotalValue(int portfolioId)
        {
            var cacheKey = $"Portfolio_{portfolioId}_PortfolioTotalValue"; 
            var cachedValues = await _cache.GetAsync<PortfolioTotalValue>(cacheKey);

            if (cachedValues != null)
            {
                return cachedValues;
            }

            var portfolioValue = await _portfolioService.GetPortfolioTotalValue(portfolioId);

            await _cache.SetAsync(cacheKey, portfolioValue, TimeSpan.FromMinutes(5));

            return portfolioValue;
        }

        public async Task<PortfolioDailyChange> GetPortfolioDailyChange(int portfolioId)
        {  
            var cacheKey = $"Portfolio_{portfolioId}_PortfolioDailyChange";
            var cachedChange = await _cache.GetAsync<PortfolioDailyChange>(cacheKey);

            if (cachedChange != null)
            {
                return cachedChange;
            }

            var dailyChange = await _portfolioService.GetPortfolioDailyChange(portfolioId);

            await _cache.SetAsync(cacheKey, dailyChange, TimeSpan.FromMinutes(5));

            return dailyChange;
            
            
        }

        public async Task<Portfolio> DeleteAsync(AppUser user, int portfolioId)
        {
            
            var portfolio = await _portfolioService.DeleteAsync(user, portfolioId);

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

        public async Task<List<PortfolioDto>> GetAllUserPortfolios(string userId)
        {
            var cacheKey = $"User_{userId}_Portfolios";
            var cachedData = await _cache.GetAsync<List<PortfolioDto>>(cacheKey);

            if (cachedData != null)
            {
                return cachedData;
            }

            var portfolioDtos = await _portfolioService.GetAllUserPortfolios(userId);

            await _cache.SetAsync(cacheKey, portfolioDtos, TimeSpan.FromMinutes(5));

            return portfolioDtos;
        }

        public async Task<PortfolioDto> UpdateNameAsync(AppUser user, int portfolioId, string newPortfolioName)
        {
            var updatedPortfolio = await _portfolioService.UpdateNameAsync(user, portfolioId, newPortfolioName);

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

        public Task<List<Portfolio>> GetAllPortfoliosAsync()
        {
            var portfolios = _portfolioService.GetAllPortfoliosAsync();
            return portfolios;
        }
    }
}
