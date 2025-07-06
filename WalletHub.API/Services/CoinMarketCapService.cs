using WalletHub.API.Dtos.Currency;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using WalletHub.API.Exceptions;

namespace WalletHub.API.Service;

public class CoinMarketCapService : ICoinMarketCapService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly string _apiKey;


    public CoinMarketCapService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
        _apiKey = config["CMPKey"];
    }

    public async Task<MarketCurrency?> FindCurrencyBySymbolAsync(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
        }
        
        var queryParams = new Dictionary<string, string>
        {
            ["symbol"] = symbol
        };
        
        var response = await ExecuteApiRequestAsync<CMPQuoteResponse>("cryptocurrency/quotes/latest", queryParams);
        
        if (response?.Data is null || !response.Data.Any())      
            throw new MarketCurrencyNotFoundException(symbol);
        
        var currencyData = response.Data[symbol];

        if (currencyData is null)    
            throw new MarketCurrencyNotFoundException(symbol);
               
        return new MarketCurrency
        {
            Name = currencyData.Name,
            Symbol = currencyData.Symbol,
            Price = currencyData.Quote?.USD?.price ?? 0,
            MarketCap = currencyData.Quote?.USD?.market_cap ?? 0,
            Volume24h = currencyData.Quote?.USD?.volume_24h ?? 0,
            PercentChange24h = currencyData.Quote?.USD?.percent_change_24h ?? 0
        };
    }

    public async Task<List<MarketCurrency>?> GetPopularCurrenciesAsync(int limit = 10)
    {
        if (limit <= 0 || limit > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be between 1 and 100");
        }
        
        var queryParams = new Dictionary<string, string>
        {
            ["limit"] = limit.ToString()
        };
        
        var response = await ExecuteApiRequestAsync<CMPListingsResponse>("cryptocurrency/listings/latest", queryParams);
        
        if (response?.Data is null || !response.Data.Any())
        {
            return null;
        }
        
        return response.Data.Select(item => new MarketCurrency
        {
            Id = item.Id,
            Name = item.Name,
            Symbol = item.Symbol,
            Price = item.Quote?.USD?.price ?? 0,
            MarketCap = item.Quote?.USD?.market_cap ?? 0,
            Volume24h = item.Quote?.USD?.volume_24h ?? 0,
            PercentChange24h = item.Quote?.USD?.percent_change_24h ?? 0,
            
        }).ToList();
        
    }

    

    public async Task<T?> ExecuteApiRequestAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var queryString = "";
            if (queryParams != null && queryParams.Count > 0)
            {
                queryString = "?" + string.Join("&", queryParams.Select(p => 
                    $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            }
            
            var requestUrl = $"https://pro-api.coinmarketcap.com/v1/{endpoint}{queryString}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);
            request.Headers.Add("Accept", "application/json");
            
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var response = await _httpClient.SendAsync(request, cts.Token);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (HttpRequestException ex)
        {
            return default;
        }
        catch (JsonException ex)
        {
            return default;
        }
        catch (TaskCanceledException ex)
        {
            return default;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<decimal?> GetCurrentPrice(string symbol)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "symbol", symbol },
            { "convert", "USD" }
        };


        var result = await ExecuteApiRequestAsync<CMPListingsResponse>("cryptocurrency/listings/latest", queryParams);

        if (result?.Data != null && result.Data.Count > 0)
        {
            var price = result.Data[0]?.Quote?.USD?.price;
            return price;
        }

        return null;
    }
}
