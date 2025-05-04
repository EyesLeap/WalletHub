using api.Dtos.Currency;
using api.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace api.Caching
{
    public class CachedCoinMarketCapService : ICoinMarketCapService
    {
        private readonly ICoinMarketCapService _inner;
        private readonly ICacheService _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(3);

        public CachedCoinMarketCapService(ICoinMarketCapService inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<MarketCurrency?> FindCurrencyBySymbolAsync(string symbol)
        {
            var cacheKey = $"currency:{symbol}";

            var cachedCurrency = await _cache.GetAsync<MarketCurrency>(cacheKey);
            if (cachedCurrency != null)
            {
                return cachedCurrency;
            }

            var currency = await _inner.FindCurrencyBySymbolAsync(symbol);
            if (currency != null)
            {
                await _cache.SetAsync(cacheKey, currency, _cacheDuration);
            }

            return currency;
        }

        public async Task<List<MarketCurrency>?> GetPopularCurrenciesAsync(int limit = 10)
        {
            return await _inner.GetPopularCurrenciesAsync(limit);
        }
    }

}