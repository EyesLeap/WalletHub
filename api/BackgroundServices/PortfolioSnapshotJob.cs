using api.Dtos.Comment;
using api.Dtos.AssetDtos;
using api.Models;
using api.Interfaces;

namespace api.BackgroundServices
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