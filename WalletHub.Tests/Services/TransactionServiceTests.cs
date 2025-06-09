using WalletHub.API.Caching;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;

namespace WalletHub.Tests
{
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
            var cancellationToken = CancellationToken.None;

            _portfolioRepoMock.Setup(x => x.GetById(portfolioId, cancellationToken)).ReturnsAsync(portfolio);
            _cmpServiceMock.Setup(x => x.FindCurrencyBySymbolAsync("BTC")).ReturnsAsync(currency);
            _assetRepoMock.Setup(x => x.GetByPortfolioAndSymbol(portfolioId, "BTC")).ReturnsAsync((Asset?)null);
            _assetRepoMock.Setup(x => x.AddAsync(It.IsAny<Asset>())).ReturnsAsync(createdAsset);
            _transactionRepoMock.Setup(x => x.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync(new Transaction());
            _cacheMock.Setup(x => x.RemoveAsync(It.IsAny<string>(), default))
            .Returns(Task.CompletedTask);

            var result = await _service.CreateTransactionAsync(portfolioId, dto, cancellationToken);
            
            Assert.NotNull(result);  
            Assert.Equal(portfolioId, result.PortfolioId);
            _transactionRepoMock.Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Once);  
        }

        // [Fact]
        // public async Task GetAllTransactionsAsync_ReturnsPagedList_WithoutFilters()
        // {
        //     var portfolioId = 1;
        //     var queryDto = new TransactionQueryDto
        //     {
        //         Page = 1,
        //         PageSize = 10,
        //         SortDescending = false
        //     };

        //      var transactions = new List<Transaction>
        //     {
        //         new Transaction { TransactionType = TransactionType.Buy, CreatedAt = DateTime.Now.AddMinutes(-10), Asset = new Asset { Symbol = "BTC" } },
        //         new Transaction { TransactionType = TransactionType.Sell, CreatedAt = DateTime.Now.AddMinutes(-5), Asset = new Asset { Symbol = "ETH" } }
        //     };

        //     var queryable = transactions.AsQueryable();

        //     _transactionRepoMock.Setup(repo => repo.GetAllByPortfolioId(portfolioId)).Returns(queryable);



        //     var result = await _service.GetAllTransactionsAsync(portfolioId, queryDto);

        //     Assert.NotNull(result);
        //     Assert.Equal(2, result.Count);
        //     Assert.Equal(1, result.CurrentPage);
        //     Assert.Equal(10, result.PageSize);
        // }

        [Fact]
        public async Task DeleteAsync_TransactionTypeBuy_UpdatesAssetAndRemovesTransaction()
        {
            var transactionId = 1;
            var asset = new Asset 
            {
                Amount = 6,
                AveragePurchasePrice = 60,
                Symbol = "BTC"  
            };
            var transaction = new Transaction
            {
                Id = transactionId,
                TransactionType = TransactionType.Buy,
                Amount = 2,
                PricePerCoin = 90,
                Asset = asset,
                PortfolioId = 1
            };

            _transactionRepoMock.Setup(repo => repo.GetByIdAsync(transactionId)).ReturnsAsync(transaction);
            _transactionRepoMock.Setup(repo => repo.DeleteAsync(transactionId)).ReturnsAsync(transaction);
            _assetRepoMock.Setup(repo => repo.UpdateAsync(It.IsAny<Asset>())).ReturnsAsync(asset);

            var result = await _service.DeleteAsync(transactionId);

            Assert.NotNull(result); 
            Assert.Equal(transactionId, result?.Id);
            _assetRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Asset>()), Times.Once);  
            _cacheMock.Verify(cache => cache.RemoveAsync($"Portfolio_{transaction.PortfolioId}_PortfolioTotalValue", CancellationToken.None));  
            
           
            Assert.Equal(45, asset.AveragePurchasePrice); 
        }



    }
}