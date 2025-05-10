using WalletHub.API.Dtos.Currency;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface ICoinMarketCapService
    {
        Task<MarketCurrency> FindCurrencyBySymbolAsync(string symbol);
        Task<List<MarketCurrency>?> GetPopularCurrenciesAsync(int limit = 10);
        Task<T?> ExecuteApiRequestAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null);
    }
}