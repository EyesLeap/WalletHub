using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using WalletHub.API.Caching;
using WalletHub.API.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using WalletHub.API.Exceptions;
using System.Net;

namespace WalletHub.API.Service
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly ICoinMarketCapService _cmpService;
        private readonly ICacheService _cache;
        private readonly IAssetRepository _assetRepository;
        private readonly IPortfolioSnapshotService _snapshotService;

        public PortfolioService(IPortfolioRepository portfolioRepository,
        IPortfolioSnapshotService snapshotService,
        UserManager<AppUser> userManager,
        ICoinMarketCapService cmpService,
        IAssetRepository assetRepository,
        ICacheService cache)
        {
            _portfolioRepository = portfolioRepository;
            _cmpService = cmpService;
            _assetRepository = assetRepository;
            _cache = cache;
            _snapshotService = snapshotService;
        }

        public async Task<List<Asset>> GetUserAssets(AppUser user, int portfolioId)
        {
            var portfolio = await _portfolioRepository.GetById(portfolioId);

            return portfolio.Assets.ToList();
        }

        public async Task<PortfolioDto> CreatePortfolio(AppUser user, CreatePortfolioDto dto)
        {
            var existingPortfolio = await _portfolioRepository.GetByNameAsync(user.Id, dto.Name);
            if (existingPortfolio != null)  
            {
                throw new PortfolioAlreadyExistsException(dto.Name);
            }

            var portfolio = dto.ToPortfolioFromCreateDto(user.Id);

            try
            {
                await _portfolioRepository.AddAsync(portfolio);
            }
            catch (Exception ex)
            {
                throw new WalletHubException("Failed to save the portfolio to the database.", HttpStatusCode.InternalServerError);
            }

            return portfolio.ToPortfolioDto();
        }


        public async Task<Portfolio> GetPortfolioById(int portfolioId)
        {
            return await _portfolioRepository.GetById(portfolioId);
        }
        
        public async Task<PortfolioTotalValue> GetPortfolioTotalValue(int portfolioId)
        {  
            var portfolio = await GetPortfolioById(portfolioId);
            if (portfolio == null) return null;
            var assets = await _assetRepository.GetAllByPortfolioIdAsync(portfolioId);
            var values = new List<AssetValue>();
            foreach (var asset in assets)
            {
                var currency = await _cmpService.FindCurrencyBySymbolAsync(asset.Symbol);
                if (currency == null) continue;
                
                values.Add(new AssetValue
                {
                    Symbol = asset.Symbol,
                    Name = asset.Name,
                    Price = currency.Price,
                    Amount = asset.Amount,
                    TotalValue = currency.Price * asset.Amount,
                    AveragePurchasePrice = asset.AveragePurchasePrice,
                    PercentChange1h = currency.PercentChange1h,
                    PercentChange24h = currency.PercentChange24h,
                    PercentChange7d = currency.PercentChange7d,
                    ProfitLoss = (currency.Price - asset.AveragePurchasePrice) * asset.Amount
                    
                    
                });
            }

            return new PortfolioTotalValue
            {
                TotalValueUSD = values.Sum(a => a.TotalValue),
                AssetValues = values
            };
        }

        public async Task<PortfolioDailyChange> GetPortfolioDailyChange(int portfolioId)
        {
            var cacheKey = $"Portfolio_{portfolioId}_PortfolioTotalValue"; 
            var portfolioTotalValue = await _cache.GetAsync<PortfolioTotalValue>(cacheKey);

            if (portfolioTotalValue == null)
            {
                portfolioTotalValue = await GetPortfolioTotalValue(portfolioId);
            }

            var snapshot = await _snapshotService.GetLatestDailySnapshot(portfolioId);

            if (snapshot == null || snapshot.TotalValueUSD == 0)
            {
                return new PortfolioDailyChange
                {
                    PercentChange24h = 0,
                    ProfitLoss = 0
                };
            }

            var profitLoss = portfolioTotalValue.TotalValueUSD - snapshot.TotalValueUSD;
            var percentChange = (profitLoss / snapshot.TotalValueUSD) * 100;

            return new PortfolioDailyChange
            {
                PercentChange24h = Math.Round(percentChange, 2),
                ProfitLoss = Math.Round(profitLoss, 2)
            };
        }


        public async Task<Portfolio> DeleteAsync(AppUser user, int portfolioId)
        {
            return await _portfolioRepository.DeleteAsync(user, portfolioId);
        }

        public async Task<List<PortfolioDto>> GetAllUserPortfolios(string userId)
        {
            var portfolios = await _portfolioRepository.GetAllUserPortfolios(userId);
            var portfolioDtos = portfolios.ToPortfolioDtos();

            foreach (var p in portfolioDtos)
            {
                var total = await GetPortfolioTotalValue(p.Id);
                p.TotalValueUSD = total.TotalValueUSD;
            }

            return portfolioDtos;
        }

        public async Task<PortfolioDto> UpdateNameAsync(AppUser user, int portfolioId, string newPortfolioName)
        {
            var portfolio = await _portfolioRepository.UpdateNameAsync(user, portfolioId, newPortfolioName);
            return portfolio.ToPortfolioDto();
        }

        public async Task<List<Portfolio>> GetAllPortfoliosAsync()
        {
            return await _portfolioRepository.GetAllPortfoliosAsync();
        }

       
  }
}