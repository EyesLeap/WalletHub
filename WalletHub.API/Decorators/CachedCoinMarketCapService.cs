using WalletHub.API.Dtos.Currency;
using WalletHub.API.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace WalletHub.API.Caching
{
    public class CachedCoinMarketCapService : ICoinMarketCapService
    {
        private readonly ICoinMarketCapService _inner;
        private readonly ICacheService _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(3);
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public CachedCoinMarketCapService(ICoinMarketCapService inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<MarketCurrency?> FindCurrencyBySymbolAsync(string symbol)
        {
            var cacheKey = $"currency:{symbol}";

            var cachedCurrency = await _cache.GetAsync<MarketCurrency>(cacheKey);
            if (cachedCurrency is not null)
            {
                return cachedCurrency;
            }

            var currency = await _inner.FindCurrencyBySymbolAsync(symbol);
            if (currency is not null)
            {
                await _cache.SetAsync(cacheKey, currency, _cacheDuration);
            }

            return currency;
        }

        // public async Task<List<MarketCurrency>?> GetPopularCurrenciesAsync(int limit = 10)
        // {
        //     var cacheKey = $"popular_currencies_{limit}";
        //     var cachedData = await _cache.GetAsync<List<MarketCurrency>>(cacheKey);
        //     if (cachedData is not null)
        //         return cachedData;

        //     var popularCurrencies = await _inner.GetPopularCurrenciesAsync(limit);
        //     if (popularCurrencies is not null)
        //     {
        //         await _cache.SetAsync(cacheKey, popularCurrencies, _cacheDuration);
        //     }
        //     return popularCurrencies;

            
        // }

        public async Task<List<MarketCurrency>?> GetPopularCurrenciesAsync(int limit = 10)
        {
            var cacheKey = $"popular_currencies_{limit}";
            var cachedData = await _cache.GetAsync<List<MarketCurrency>>(cacheKey);
            if (cachedData is not null)
                return cachedData;

            var semaphore = _locks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                cachedData = await _cache.GetAsync<List<MarketCurrency>>(cacheKey);
                if (cachedData is not null)
                    return cachedData;

                var popularCurrencies = await _inner.GetPopularCurrenciesAsync(limit);
                if (popularCurrencies is not null)
                    await _cache.SetAsync(cacheKey, popularCurrencies, _cacheDuration);

                return popularCurrencies;
            }
            finally
            {
                semaphore.Release();
            }      
            
        }
        
        public async Task<T?> ExecuteApiRequestAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null)
        {
            return await _inner.ExecuteApiRequestAsync<T>(endpoint, queryParams);
        }
    }

}