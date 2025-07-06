using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Threading.Tasks;
using WalletHub.API.Caching;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Exceptions;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Xunit;

namespace WalletHub.Tests.UnitTests.Services
{
    public class CoinMarketCapServiceTests : IDisposable
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly ICoinMarketCapService _service;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public CoinMarketCapServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var config = new Mock<IConfiguration>();
            config.Setup(c => c["CMPKey"]).Returns("api-key");

            _service = new CoinMarketCapService(httpClient, config.Object);
        }

        [Fact]
        public async Task FindCurrencyBySymbolAsync_ReturnsCurrency_WhenDataIsValid()
        {
            var symbol = "BTC";
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\":{\"BTC\":{\"name\":\"Bitcoin\",\"symbol\":\"BTC\",\"quote\":{\"USD\":{\"price\":50000}}}}}")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var result = await _service.FindCurrencyBySymbolAsync(symbol);

            Assert.NotNull(result);
            Assert.Equal("Bitcoin", result.Name);
            Assert.Equal("BTC", result.Symbol);
            Assert.Equal(50000m, result.Price);
        }

        [Fact]
        public async Task GetPopularCurrenciesAsync_ConcurrentCalls_ShouldCallInnerServiceOnlyOnce()
        {
            var limit = 5;
            var expectedCurrencies = new List<MarketCurrency>
            {
                new() { Symbol = "BTC", Name = "Bitcoin", Price = 50000m },
                new() { Symbol = "ETH", Name = "Ethereum", Price = 3000m }
            };

            var cacheMock = new Mock<ICacheService>();
            var cacheStorage = new Dictionary<string, List<MarketCurrency>>();

            cacheMock.Setup(x => x.GetAsync<List<MarketCurrency>>(It.IsAny<string>()))
                    .ReturnsAsync((string key) => cacheStorage.TryGetValue(key, out var value) ? value : null);

            cacheMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<MarketCurrency>>(), It.IsAny<TimeSpan>()))
                    .Callback<string, List<MarketCurrency>, TimeSpan>((key, value, _) => cacheStorage[key] = value)
                    .Returns(Task.CompletedTask);

            var innerServiceMock = new Mock<ICoinMarketCapService>();
            var callCount = 0;
            
            innerServiceMock.Setup(x => x.GetPopularCurrenciesAsync(limit))
                            .Returns(async () =>
                            {
                                Interlocked.Increment(ref callCount);
                                await Task.Delay(200);
                                return expectedCurrencies;
                            });

            var cachedService = new CachedCoinMarketCapService(innerServiceMock.Object, cacheMock.Object);

            var tasks = Enumerable.Range(0, 5)
                                .Select(_ => cachedService.GetPopularCurrenciesAsync(limit))
                                .ToArray();

            var results = await Task.WhenAll(tasks);

            Assert.All(results, result => 
            {
                Assert.NotNull(result);
                Assert.Equal(expectedCurrencies.Count, result.Count);
                Assert.Equal("BTC", result[0].Symbol);
                Assert.Equal("ETH", result[1].Symbol);
            });

            Assert.Equal(1, callCount);
            innerServiceMock.Verify(x => x.GetPopularCurrenciesAsync(limit), Times.Once);
            
            cacheMock.Verify(x => x.SetAsync(
                $"popular_currencies_{limit}", 
                It.IsAny<List<MarketCurrency>>(), 
                It.IsAny<TimeSpan>()), Times.Once);
        }

        public void Dispose()
        {
            
        }

    }


}
