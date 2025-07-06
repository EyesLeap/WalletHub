using WalletHub.API.Caching;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Moq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using FluentAssertions;
using WalletHub.API.Exceptions;
using MockQueryable.Moq;

namespace WalletHub.Tests.UnitTests.Services
{
    public class TransactionServiceTests : IDisposable
    {
        private readonly Mock<IPortfolioRepository> _portfolioRepositoryMock;
        private readonly Mock<IAssetRepository> _assetRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<ICoinMarketCapService> _coinMarketCapServiceMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly TransactionService _sut; 
        private readonly CancellationToken _cancellationToken;

        public TransactionServiceTests()
        {
            _portfolioRepositoryMock = new Mock<IPortfolioRepository>();
            _assetRepositoryMock = new Mock<IAssetRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _coinMarketCapServiceMock = new Mock<ICoinMarketCapService>();
            _distributedCacheMock = new Mock<IDistributedCache>();
            _cancellationToken = CancellationToken.None;

            _sut = new TransactionService(
                _portfolioRepositoryMock.Object,
                _assetRepositoryMock.Object,
                _transactionRepositoryMock.Object,
                _coinMarketCapServiceMock.Object,
                _distributedCacheMock.Object
            );
        }

        public class CreateTransactionAsyncTests : TransactionServiceTests
        {
            [Fact]
            public async Task CreateTransactionAsync_WhenPortfolioNotFound_ShouldThrowNotFoundException()
            {   
                // Arrange
                var portfolioId = 1;
                var createTransactionDto = CreateBuyTransactionDto();

                _portfolioRepositoryMock
                    .Setup(x => x.GetById(portfolioId, _cancellationToken))
                    .ReturnsAsync(null as Portfolio);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<PortfolioNotFoundException>(
                    () => _sut.CreateTransactionAsync(portfolioId, createTransactionDto, _cancellationToken));

                exception.Message.Should().Contain("not found");
            }

            [Fact]
            public async Task CreateTransactionAsync_WhenCurrencyNotFound_ShouldThrowNotFoundException()
            {
                // Arrange
                var portfolioId = 1;
                var portfolio = CreatePortfolio(portfolioId);
                var createTransactionDto = CreateBuyTransactionDto();

                _portfolioRepositoryMock
                    .Setup(x => x.GetById(portfolioId, _cancellationToken))
                    .ReturnsAsync(portfolio);

                _coinMarketCapServiceMock
                    .Setup(x => x.FindCurrencyBySymbolAsync(createTransactionDto.Symbol))
                    .ReturnsAsync(null as MarketCurrency);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<MarketCurrencyNotFoundException>(
                    () => _sut.CreateTransactionAsync(portfolioId, createTransactionDto, _cancellationToken));

                exception.Message.Should().Contain("not found");
            }

            [Fact]
            public async Task CreateTransactionAsync_WhenAddingNewAsset_ShouldCreateAssetAndTransaction()
            {
                // Arrange
                var portfolioId = 1;
                var portfolio = CreatePortfolio(portfolioId);
                var createTransactionDto = CreateBuyTransactionDto("BTC", 1m, 30000m);
                var marketCurrency = CreateMarketCurrency("BTC", "Bitcoin");
                var createdAsset = CreateAsset("BTC", 1m, 30000m, portfolioId);
                var createdTransaction = CreateTransaction(TransactionType.Buy, 1m, 30000m, createdAsset, portfolioId);

                SetupSuccessfulCreateTransaction(portfolioId, portfolio, marketCurrency, createdAsset, createdTransaction);

                _assetRepositoryMock
                    .Setup(x => x.GetByPortfolioAndSymbol(portfolioId, "BTC"))
                    .ReturnsAsync(null as Asset);

                // Act
                var result = await _sut.CreateTransactionAsync(portfolioId, createTransactionDto, _cancellationToken);

                // Assert
                result.Should().NotBeNull();
                result.PortfolioId.Should().Be(portfolioId);

                _assetRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Asset>()), Times.Once);
                _transactionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transaction>()), Times.Once);
                VerifyCacheInvalidation(portfolioId);
            }

            [Fact]
            public async Task CreateTransactionAsync_WhenAssetExists_ShouldUpdateExistingAsset()
            {
                // Arrange
                var portfolioId = 1;
                var portfolio = CreatePortfolio(portfolioId);
                var existingAsset = CreateAsset("BTC", 5m, 25000m, portfolioId);
                var createTransactionDto = CreateBuyTransactionDto("BTC", 2m, 35000m);
                var marketCurrency = CreateMarketCurrency("BTC", "Bitcoin");
                var createdTransaction = CreateTransaction(TransactionType.Buy, 2m, 35000m, existingAsset, portfolioId);

                SetupSuccessfulCreateTransaction(portfolioId, portfolio, marketCurrency, existingAsset, createdTransaction);

                _assetRepositoryMock
                    .Setup(x => x.GetByPortfolioAndSymbol(portfolioId, "BTC"))
                    .ReturnsAsync(existingAsset);

                // Act
                var result = await _sut.CreateTransactionAsync(portfolioId, createTransactionDto, _cancellationToken);

                // Assert
                result.Should().NotBeNull();
                result.PortfolioId.Should().Be(portfolioId);

                _assetRepositoryMock.Verify(x => x.UpdateAsync(existingAsset), Times.Once);
                _assetRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Asset>()), Times.Never);

                // Verify asset was updated with correct values
                var expectedAmount = 5m + 2m; // 7m
                var expectedAverage = ((5m * 25000m) + (2m * 35000m)) / 7m; // 27142.86

                existingAsset.Amount.Should().Be(expectedAmount);
                existingAsset.AveragePurchasePrice.Should().BeApproximately(expectedAverage, 0.01m);
            }

            [Fact]
            public async Task CreateTransactionAsync_WhenSellTransaction_ShouldNotChangeAveragePrice()
            {
                // Arrange
                var portfolioId = 1;
                var expectedAssetValue = 10m - 3m;
                var assetAveragePrice = 30000m;
                var portfolio = CreatePortfolio(portfolioId);
                var existingAsset = CreateAsset("BTC", 10m, assetAveragePrice, portfolioId);
                var createTransactionDto = CreateSellTransactionDto("BTC", 3m, 35000m);
                var marketCurrency = CreateMarketCurrency("BTC", "Bitcoin");
                var createdTransaction = CreateTransaction(TransactionType.Sell, 3m, 35000m, existingAsset, portfolioId);
               

                SetupSuccessfulCreateTransaction(portfolioId, portfolio, marketCurrency, existingAsset, createdTransaction);

                _assetRepositoryMock
                    .Setup(x => x.GetByPortfolioAndSymbol(portfolioId, "BTC"))
                    .ReturnsAsync(existingAsset);

                // Act
                var result = await _sut.CreateTransactionAsync(portfolioId, createTransactionDto, _cancellationToken);

                // Assert
                result.Should().NotBeNull();

                existingAsset.AveragePurchasePrice.Should().Be(assetAveragePrice); // Should remain unchanged
                existingAsset.Amount.Should().Be(expectedAssetValue); 
            }

            private void SetupSuccessfulCreateTransaction(int portfolioId, Portfolio portfolio,
                MarketCurrency marketCurrency, Asset asset, Transaction transaction)
            {
                _portfolioRepositoryMock
                    .Setup(x => x.GetById(portfolioId, _cancellationToken))
                    .ReturnsAsync(portfolio);

                _coinMarketCapServiceMock
                    .Setup(x => x.FindCurrencyBySymbolAsync(marketCurrency.Symbol))
                    .ReturnsAsync(marketCurrency);

                _assetRepositoryMock
                    .Setup(x => x.AddAsync(It.IsAny<Asset>()))
                    .ReturnsAsync(asset);

                _assetRepositoryMock
                    .Setup(x => x.UpdateAsync(It.IsAny<Asset>()))
                    .ReturnsAsync(asset);

                _transactionRepositoryMock
                    .Setup(x => x.AddAsync(It.IsAny<Transaction>()))
                    .ReturnsAsync(transaction);

                _distributedCacheMock
                    .Setup(x => x.RemoveAsync(It.IsAny<string>(), _cancellationToken))
                    .Returns(Task.CompletedTask);
            }
        }

        public class DeleteAsyncTests : TransactionServiceTests
        {
            [Fact]
            public async Task DeleteAsync_WhenTransactionNotFound_ShouldThrowNotFoundException()
            {
                // Arrange
                var transactionId = 1;

                _transactionRepositoryMock
                    .Setup(x => x.GetByIdAsync(transactionId))
                    .ReturnsAsync((Transaction?)null);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<TransactionNotFoundException>(
                    () => _sut.DeleteAsync(transactionId));

                exception.Message.Should().Contain("not found");
            }

            [Fact]
            public async Task DeleteAsync_WhenDeletingBuyTransaction_ShouldReverseAssetChanges()
            {
                // Arrange
                var transactionId = 1;
                var portfolioId = 1;
                var asset = CreateAsset("BTC", 6m, 60m, portfolioId);
                var transaction = CreateTransaction(TransactionType.Buy, 2m, 90m, asset, portfolioId);
                transaction.Id = transactionId;

                SetupSuccessfulDelete(transactionId, transaction, asset);

                // Act
                var result = await _sut.DeleteAsync(transactionId);

                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be(transactionId);

                // Verify asset was updated correctly (reverse buy transaction)
                var expectedAmount = 6m - 2m; // 4m
                var expectedAverage = ((6m * 60m) - (2m * 90m)) / 4m; // 45m

                asset.Amount.Should().Be(expectedAmount);
                asset.AveragePurchasePrice.Should().Be(expectedAverage);

                VerifyDeleteCalls(transactionId, asset, portfolioId);
            }

            [Fact]
            public async Task DeleteAsync_WhenDeletingSellTransaction_ShouldReverseAssetChanges()
            {
                // Arrange
                var transactionId = 1;
                var portfolioId = 1;
                var asset = CreateAsset("BTC", 6m, 60m, portfolioId);
                var transaction = CreateTransaction(TransactionType.Sell, 2m, 90m, asset, portfolioId);
                transaction.Id = transactionId;

                SetupSuccessfulDelete(transactionId, transaction, asset);

                // Act
                var result = await _sut.DeleteAsync(transactionId);

                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be(transactionId);

                // Verify asset was updated correctly (reverse sell transaction)
                var expectedAmount = 6m + 2m; 
                var expectedAverage = 60m; // Average price remains unchanged for sell reversal

                asset.Amount.Should().Be(expectedAmount);
                asset.AveragePurchasePrice.Should().Be(expectedAverage);

                VerifyDeleteCalls(transactionId, asset, portfolioId);
            }

            private void SetupSuccessfulDelete(int transactionId, Transaction transaction, Asset asset)
            {
                _transactionRepositoryMock
                    .Setup(x => x.GetByIdAsync(transactionId))
                    .ReturnsAsync(transaction);

                _transactionRepositoryMock
                    .Setup(x => x.DeleteAsync(transactionId))
                    .ReturnsAsync(transaction);

                _assetRepositoryMock
                    .Setup(x => x.UpdateAsync(It.IsAny<Asset>()))
                    .ReturnsAsync(asset);

                _distributedCacheMock
                    .Setup(x => x.RemoveAsync(It.IsAny<string>(), _cancellationToken))
                    .Returns(Task.CompletedTask);
            }

            private void VerifyDeleteCalls(int transactionId, Asset asset, int portfolioId)
            {
                _transactionRepositoryMock.Verify(x => x.DeleteAsync(transactionId), Times.Once);
                _assetRepositoryMock.Verify(x => x.UpdateAsync(asset), Times.Once);
                VerifyCacheInvalidation(portfolioId);
            }
        }

        public class GetAllTransactionsAsyncTests : TransactionServiceTests
        {
            [Fact]
            public async Task GetAllTransactionsAsync_WhenNoFilters_ShouldReturnPagedResults()
            {
                // Arrange
                var portfolioId = 1;
                var queryDto = new TransactionQueryDto
                {
                    Page = 1,
                    PageSize = 10,
                    SortDescending = false
                };

                var transactions = CreateTransactionList(portfolioId, 5);

                _transactionRepositoryMock
                    .Setup(x => x.GetAllByPortfolioId(portfolioId))
                    .Returns(transactions.AsQueryable().BuildMockDbSet().Object);

                // Act
                var result = await _sut.GetAllTransactionsAsync(portfolioId, queryDto);

                // Assert
                result.Should().NotBeNull();
                result.TotalPages.Should().Be(1);
                result.CurrentPage.Should().Be(1);
                result.PageSize.Should().Be(10);
                result.TotalCount.Should().Be(5);
            }

            [Fact]
            public async Task GetAllTransactionsAsync_WhenFiltersApplied_ShouldReturnFilteredResults()
            {
                // Arrange
                var portfolioId = 1;
                var queryDto = new TransactionQueryDto
                {
                    Page = 1,
                    PageSize = 10,
                    AssetSymbol = "BTC",
                    TransactionType = TransactionType.Buy
                };

                var btcAsset = CreateAsset("BTC", 5m, 30000m, portfolioId);
                var ethAsset = CreateAsset("ETH", 10m, 2000m, portfolioId);

                var transactions = new List<Transaction>
                {
                    CreateTransaction(TransactionType.Buy, 1m, 30000m, btcAsset, portfolioId),
                    CreateTransaction(TransactionType.Buy, 2m, 2000m, ethAsset, portfolioId),
                    CreateTransaction(TransactionType.Sell, 0.5m, 35000m, btcAsset, portfolioId)
                };

                _transactionRepositoryMock
                    .Setup(x => x.GetAllByPortfolioId(portfolioId))
                    .Returns(transactions.AsQueryable().BuildMockDbSet().Object);

                // Act
                var result = await _sut.GetAllTransactionsAsync(portfolioId, queryDto);

                // Assert
                result.Should().NotBeNull();
                result.TotalPages.Should().Be(1);
                result.First().Symbol.Should().Be("BTC");
                result.First().TransactionType.Should().Be(TransactionType.Buy);
            }

            [Fact]
            public async Task GetAllTransactionsAsync_WhenSortDescending_ShouldReturnSortedResults()
            {
                // Arrange
                var portfolioId = 1;
                var queryDto = new TransactionQueryDto
                {
                    Page = 1,
                    PageSize = 10,
                    SortDescending = true
                };

                var asset = CreateAsset("BTC", 5m, 30000m, portfolioId);
                var transactions = new List<Transaction>
                {
                    CreateTransaction(TransactionType.Buy, 1m, 30000m, asset, portfolioId, DateTime.UtcNow.AddHours(-2)),
                    CreateTransaction(TransactionType.Buy, 2m, 31000m, asset, portfolioId, DateTime.UtcNow.AddHours(-1)),
                    CreateTransaction(TransactionType.Sell, 0.5m, 35000m, asset, portfolioId, DateTime.UtcNow)
                };

                _transactionRepositoryMock
                    .Setup(x => x.GetAllByPortfolioId(portfolioId))
                    .Returns(transactions.AsQueryable().BuildMockDbSet().Object);

                // Act
                var result = await _sut.GetAllTransactionsAsync(portfolioId, queryDto);

                // Assert
                result.Should().NotBeNull();
                result.TotalCount.Should().Be(3);
                result.First().TransactionType.Should().Be(TransactionType.Sell); // Most recent
                result.Last().PricePerCoin.Should().Be(30000m); // Oldest
            }

            private List<Transaction> CreateTransactionList(int portfolioId, int count)
            {
                var asset = CreateAsset("BTC", 10m, 30000m, portfolioId);
                var transactions = new List<Transaction>();

                for (int i = 0; i < count; i++)
                {
                    var transaction = CreateTransaction(
                        i % 2 == 0 ? TransactionType.Buy : TransactionType.Sell,
                        1m + i,
                        30000m + (i * 1000),
                        asset,
                        portfolioId,
                        DateTime.UtcNow.AddHours(-i)
                    );
                    transactions.Add(transaction);
                }

                return transactions;
            }
        }

        // Test Data Factory Methods
        private static CreateTransactionDto CreateBuyTransactionDto(string symbol = "BTC", decimal amount = 1m, decimal price = 30000m)
        {
            return new CreateTransactionDto
            {
                Symbol = symbol,
                Amount = amount,
                PricePerCoin = price,
                TransactionType = TransactionType.Buy,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static CreateTransactionDto CreateSellTransactionDto(string symbol = "BTC", decimal amount = 1m, decimal price = 30000m)
        {
            return new CreateTransactionDto
            {
                Symbol = symbol,
                Amount = amount,
                PricePerCoin = price,
                TransactionType = TransactionType.Sell,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static Portfolio CreatePortfolio(int id = 1, string name = "Test Portfolio")
        {
            return new Portfolio
            {
                Id = id,
                Name = name,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static Asset CreateAsset(string symbol = "BTC", decimal amount = 5m, decimal avgPrice = 25000m, int portfolioId = 1)
        {
            return new Asset
            {
                Id = new Random().Next(1, 1000),
                Symbol = symbol,
                Amount = amount,
                AveragePurchasePrice = avgPrice,
                PortfolioId = portfolioId
            };
        }

        private static Transaction CreateTransaction(TransactionType type, decimal amount, decimal price, Asset asset, int portfolioId, DateTime? createdAt = null)
        {
            return new Transaction
            {
                Id = new Random().Next(1, 1000),
                TransactionType = type,
                Amount = amount,
                PricePerCoin = price,
                Asset = asset,
                AssetId = asset.Id,
                PortfolioId = portfolioId,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        private static MarketCurrency CreateMarketCurrency(string symbol = "BTC", string name = "Bitcoin")
        {
            return new MarketCurrency
            {
                Symbol = symbol,
                Name = name,
                Price = 30000m,
                MarketCap = 600000000000m,
                Volume24h = 3500000000m,
                PercentChange24h = 5m
            };
        }

        private void VerifyCacheInvalidation(int portfolioId)
        {
            _distributedCacheMock.Verify(
                x => x.RemoveAsync($"Portfolio_{portfolioId}_PortfolioTotalValue", _cancellationToken),
                Times.Once);
        }

        public void Dispose()
        {
            
        }
    }
}