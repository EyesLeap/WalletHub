using WalletHub.API.Caching;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace WalletHub.Tests;

public class TransactionServiceTests
{
    private readonly Mock<IPortfolioRepository> _portfolioRepoMock = new();
    private readonly Mock<IAssetRepository> _assetRepoMock = new();
    private readonly Mock<ITransactionRepository> _transactionRepoMock = new();
    private readonly Mock<ICoinMarketCapService> _cmpServiceMock = new();
    private readonly Mock<IDistributedCache> _cacheMock = new();

    private readonly TransactionService _service;

    public TransactionServiceTests()    
    {
        _service = new TransactionService(
            _portfolioRepoMock.Object,
            _assetRepoMock.Object,
            _transactionRepoMock.Object,
            _cmpServiceMock.Object,
            _cacheMock.Object
        );
    }

        [Fact]
        public async Task CreateTransactionAsync_AddNewAsset_Success()
        {
            int portfolioId = 1;
            var dto = new CreateTransactionDto
            {
                Symbol = "BTC",
                Amount = 1,
                PricePerCoin = 30000,
                TransactionType = TransactionType.Buy,
                CreatedAt = DateTime.UtcNow
            };

            var portfolio = new Portfolio { Id = portfolioId };
            var currency = new MarketCurrency
            {
                Name = "Bitcoin",
                Symbol = "BTC",
                Price = 30000,
                MarketCap = 600000000000,
                Volume24h = 3500000000,
                PercentChange24h = 5
            };

            var createdAsset = new Asset { Id = 99, Symbol = "BTC", PortfolioId = portfolioId };

            _portfolioRepoMock.Setup(x => x.GetById(portfolioId)).ReturnsAsync(portfolio);
            _cmpServiceMock.Setup(x => x.FindCurrencyBySymbolAsync("BTC")).ReturnsAsync(currency);
            _assetRepoMock.Setup(x => x.GetByPortfolioAndSymbol(portfolioId, "BTC")).ReturnsAsync((Asset?)null);
            _assetRepoMock.Setup(x => x.AddAsync(It.IsAny<Asset>())).ReturnsAsync(createdAsset);
            _transactionRepoMock.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync(new Transaction());
            _cacheMock.Setup(x => x.RemoveAsync(It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

            var result = await _service.CreateTransactionAsync(portfolioId, dto);
            
            Assert.NotNull(result);  
            Assert.Equal(portfolioId, result.PortfolioId);
            _transactionRepoMock.Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Once);  
        }
}
