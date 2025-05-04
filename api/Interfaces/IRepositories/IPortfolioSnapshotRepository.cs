using api.Dtos.Comment;
using api.Models;

namespace api.Interfaces
{
    public interface IPortfolioSnapshotRepository
    {
        Task<PortfolioSnapshot> AddSnapshotAsync(PortfolioSnapshot snapshot);
        IQueryable<PortfolioSnapshot> GetPortfolioSnapshotsQueryable(int portfolioId);
        Task<PortfolioSnapshot?> GetLatestSnapshotAsync(int portfolioId);
    }

}