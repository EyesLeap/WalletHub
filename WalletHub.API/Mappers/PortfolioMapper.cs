using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Models;

namespace WalletHub.API.Mappers
{
    public static class PortfolioMapper
    {
        public static PortfolioDto ToPortfolioDto(this Portfolio portfolio)
        {
            return new PortfolioDto
            {
                Id = portfolio.Id,
                Name = portfolio.Name,
                CreatedAt = portfolio.CreatedAt,
                Assets = portfolio.Assets?
                    .Select(a => a.ToGetAssetDto())
                    .ToList() ?? new List<GetAssetDto>(),

                Transactions = portfolio.Transactions?
                    .Select(t => t.ToTransactionDto())
                    .ToList() ?? new List<TransactionDto>()
            };
        }

        public static List<PortfolioDto> ToPortfolioDtos(this List<Portfolio> portfolios)
        {
            return portfolios.Select(p => p.ToPortfolioDto()).ToList();
        }
        public static Portfolio ToPortfolioFromCreateDto(this CreatePortfolioDto dto, string userId)
        {
            return new Portfolio
            {
                AppUserId = userId,
                Name = dto.Name
            };
        }
    }
}