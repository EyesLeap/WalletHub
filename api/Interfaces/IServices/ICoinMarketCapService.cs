using api.Dtos.Currency;
using api.Models;

namespace api.Interfaces
{
    public interface ICoinMarketCapService
    {
        Task<MarketCurrency> FindCurrencyBySymbolAsync(string symbol);
        Task<List<MarketCurrency>?> GetPopularCurrenciesAsync(int limit = 10);
    }
}