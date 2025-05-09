using WalletHub.API.Dtos.Comment;
using WalletHub.API.Models;

namespace WalletHub.API.Interfaces
{
    public interface IPortfolioSnapshotRepository
    {
        Task<PortfolioSnapshot> AddSnapshotAsync(PortfolioSnapshot snapshot);
        IQueryable<PortfolioSnapshot> GetPortfolioSnapshotsQueryable(int portfolioId);
        Task<PortfolioSnapshot?> GetLatestSnapshotAsync(int portfolioId);
    }

}