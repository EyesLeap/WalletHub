using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Threading.Tasks;
using WalletHub.API.Caching;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Exceptions;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Xunit;

namespace WalletHub.Tests.UnitTests
{
    public class CoinMarketCapServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly CoinMarketCapService _service;
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

    }


}
