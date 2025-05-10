using Moq;
using System.Threading.Tasks;
using WalletHub.API.Caching;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Exceptions;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Service;
using Xunit;

namespace WalletHub.Tests.UnitTests
{
    public class PortfolioServiceTests
    {
        private readonly Mock<IPortfolioRepository> _portfolioRepoMock;
        private readonly Mock<ICoinMarketCapService> _cmpServiceMock;
        private readonly Mock<IAssetRepository> _assetRepoMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly Mock<IPortfolioSnapshotService> _snapshotServiceMock;
        private readonly PortfolioService _service;

        public PortfolioServiceTests()
        {
            _portfolioRepoMock = new Mock<IPortfolioRepository>();
            _cmpServiceMock = new Mock<ICoinMarketCapService>();
            _assetRepoMock = new Mock<IAssetRepository>();
            _cacheMock = new Mock<ICacheService>();
            _snapshotServiceMock = new Mock<IPortfolioSnapshotService>();

            _service = new PortfolioService(
                _portfolioRepoMock.Object,
                _snapshotServiceMock.Object,
                null, 
                _cmpServiceMock.Object,
                _assetRepoMock.Object,
                _cacheMock.Object
            );
        }

        [Fact]
        public async Task CreatePortfolio_CreatesAndReturnsPortfolio_WhenNameIsUnique()
        {
           
            var user = new AppUser { Id = "user123" };
            var createDto = new CreatePortfolioDto { Name = "New Portfolio" };

            _portfolioRepoMock
                .Setup(x => x.GetByNameAsync(user.Id, createDto.Name))
                .ReturnsAsync((Portfolio?)null); 

            _portfolioRepoMock
                .Setup(x => x.AddAsync(It.IsAny<Portfolio>()))
                .ReturnsAsync((Portfolio p) => p);

            var expectedDto = new Portfolio
            {
                Name = createDto.Name,
                AppUserId = user.Id,
            }.ToPortfolioDto();

       
            var result = await _service.CreatePortfolio(user, createDto);

            
            Assert.Equal(expectedDto.Name, result.Name);
            Assert.Equal(expectedDto.Id, result.Id);
            _portfolioRepoMock.Verify(x => x.AddAsync(It.IsAny<Portfolio>()), Times.Once);
        }

        [Fact]
        public async Task CreatePortfolio_ThrowsException_WhenPortfolioWithSameNameExists()
        {
            var user = new AppUser { Id = "user123" };
            var createDto = new CreatePortfolioDto { Name = "Existing" };
            var existingPortfolio = new Portfolio { Name = "Existing" };

            _portfolioRepoMock
                .Setup(x => x.GetByNameAsync(user.Id, createDto.Name))
                .ReturnsAsync(existingPortfolio);

            await Assert.ThrowsAsync<PortfolioAlreadyExistsException>(
                () => _service.CreatePortfolio(user, createDto));

            _portfolioRepoMock.Verify(x => x.AddAsync(It.IsAny<Portfolio>()), Times.Never);
        }



        [Fact]
        public async Task GetPortfolioById_ReturnsCorrectPortfolio()
        {
            int portfolioId = 1;
            var expectedPortfolio = new Portfolio { Id = portfolioId, Name = "My Portfolio" };

            _portfolioRepoMock.Setup(x => x.GetById(portfolioId)).ReturnsAsync(expectedPortfolio);

            var result = await _service.GetPortfolioById(portfolioId);

            Assert.NotNull(result); 
            Assert.Equal(portfolioId, result.Id);  
            Assert.Equal("My Portfolio", result.Name);  
        }
    }
}
