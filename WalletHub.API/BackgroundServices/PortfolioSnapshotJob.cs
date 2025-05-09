using WalletHub.API.Dtos.Comment;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;
using WalletHub.API.Interfaces;

namespace WalletHub.API.BackgroundServices
{
    public class PortfolioSnapshotJob
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PortfolioSnapshotJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task CreateSnapshotsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var portfolioService = scope.ServiceProvider.GetRequiredService<IPortfolioService>();
            var portfolios = await portfolioService.GetAllPortfoliosAsync();

            foreach (var portfolio in portfolios)
            {
                using var innerScope = _scopeFactory.CreateScope();
                var innerPortfolioService = innerScope.ServiceProvider.GetRequiredService<IPortfolioService>();
                var snapshotService = innerScope.ServiceProvider.GetRequiredService<IPortfolioSnapshotService>();

                var totalValue = await innerPortfolioService.GetPortfolioTotalValue(portfolio.Id);
                await snapshotService.CreateSnapshotAsync(portfolio.Id, totalValue);
            }
        }

    }



}