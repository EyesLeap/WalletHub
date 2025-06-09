using WalletHub.API.Dtos.Currency;
using WalletHub.API.Dtos.Portfolio;
using WalletHub.API.Dtos.TransactionDtos;
using WalletHub.API.Helpers;
using WalletHub.API.Interfaces;
using WalletHub.API.Mappers;
using WalletHub.API.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using WalletHub.API.Dtos.Common;
using WalletHub.API.Exceptions;

namespace WalletHub.API.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICoinMarketCapService _cmpService;
        private readonly IDistributedCache _cache;
        public TransactionService(IPortfolioRepository portfolioRepository,
        IAssetRepository assetServiceRepository,
        ITransactionRepository transactionRepository,
        ICoinMarketCapService cmpService,
        IDistributedCache cache)
        {
            _portfolioRepository = portfolioRepository;
            _assetRepository = assetServiceRepository;
            _transactionRepository = transactionRepository;
            _cmpService = cmpService;
            _cache = cache;
        }
        public async Task<Transaction?> CreateTransactionAsync(int portfolioId, CreateTransactionDto dto, CancellationToken cancellationToken)
        { 
            var portfolio = await _portfolioRepository.GetById(portfolioId, cancellationToken);
            if (portfolio == null)
                throw new PortfolioNotFoundException(portfolioId);

            var marketCurrency = await _cmpService.FindCurrencyBySymbolAsync(dto.Symbol);
            if (marketCurrency == null) 
                throw new NotFoundException($"Market currency with symbol {dto.Symbol} was not found.");

            var asset = await _assetRepository.GetByPortfolioAndSymbol(portfolioId, dto.Symbol);

            if (asset == null && dto.TransactionType == TransactionType.Buy) 
                asset = await CreateAsset(marketCurrency, portfolioId, dto);

            bool success = await ProcessTransactionAsync(asset, dto);

            if (!success) 
                throw new WalletHubException("Transaction cannot be proceed");
                
            var transaction = CreateTransactionEntity(portfolioId, asset.Id, dto);
            await _transactionRepository.AddAsync(transaction);

            var cacheKey = $"Portfolio_{portfolioId}_PortfolioTotalValue";
            await _cache.RemoveAsync(cacheKey);
            
            return transaction;
        }
        private async Task<Asset> CreateAsset(MarketCurrency marketCurrency, int portfolioId, CreateTransactionDto dto)
        {
            
            var asset = new Asset
            {
                PortfolioId = portfolioId,
                Symbol = dto.Symbol,
                Name = marketCurrency.Name, 
                Amount = 0, 
                AveragePurchasePrice = 0 
            };
  
            return await _assetRepository.AddAsync(asset);
        }

        private async Task<bool> ProcessTransactionAsync(Asset asset, CreateTransactionDto dto)
        {
            if (asset == null) return false;
                 

            if (dto.TransactionType == TransactionType.Sell)
            {
                if (asset.Amount < dto.Amount)
                    return false; 
            }

            await UpdateAsset(asset, dto.Amount, dto.PricePerCoin, dto.TransactionType);
         
            return true;
        }

        private Transaction CreateTransactionEntity(int portfolioId, int assetId, CreateTransactionDto dto)
        {
            return new Transaction
            {
                PortfolioId = portfolioId,
                AssetId = assetId,
                Amount = dto.Amount, 
                PricePerCoin = dto.PricePerCoin,
                TotalCost = dto.Amount * dto.PricePerCoin,
                TransactionType = dto.TransactionType,
                CreatedAt = dto.CreatedAt
            };
        }
        private async Task UpdateAsset(Asset asset, decimal transactionAmount,
         decimal pricePerCoin, TransactionType transactionType)
        {
            decimal totalValueBefore = asset.AveragePurchasePrice * asset.Amount;
            decimal totalValueNew = pricePerCoin * Math.Abs(transactionAmount);

            if (transactionType == TransactionType.Buy)
            {             
                asset.Amount += transactionAmount;
                asset.AveragePurchasePrice = (totalValueBefore + totalValueNew) / asset.Amount;
            }
            else if (transactionType == TransactionType.Sell)
            {
                asset.Amount -= transactionAmount;
                if (asset.Amount == 0) asset.AveragePurchasePrice = 0;

            }

            await _assetRepository.UpdateAsync(asset);
           
        }
        



        public async Task<Transaction> GetTransactionByIdAsync(int transactionId)
        {
            return await _transactionRepository.GetByIdAsync(transactionId);
        } 

        public async Task<Transaction?> DeleteAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null) 
                throw new TransactionNotFoundException($"Transaction with id {transactionId} was not found.");
            if (transaction.TransactionType == TransactionType.Buy)
            {
                decimal totalValueBefore = transaction.Asset.AveragePurchasePrice * transaction.Asset.Amount;
                decimal totalValueNew = transaction.Amount * transaction.PricePerCoin;
    
                transaction.Asset.Amount -= transaction.Amount;
               
                if (transaction.Asset.Amount > 0)
                {       
                    transaction.Asset.AveragePurchasePrice = (totalValueBefore - totalValueNew) / transaction.Asset.Amount;
                }
                else
                {
                    transaction.Asset.AveragePurchasePrice = 0;
                }           
            }
            else if (transaction.TransactionType == TransactionType.Sell)
            {
               transaction.Asset.Amount += transaction.Amount;  
            }

            await _assetRepository.UpdateAsync(transaction.Asset);
            var cacheKey = $"Portfolio_{transaction.PortfolioId}_PortfolioTotalValue";
            await _cache.RemoveAsync(cacheKey);
            return await _transactionRepository.DeleteAsync(transactionId);
        }

        public async Task<PagedList<TransactionDto>> GetAllTransactionsAsync(int portfolioId, TransactionQueryDto queryDto)
        {
            var query = _transactionRepository.GetAllByPortfolioId(portfolioId);

            if (queryDto.TransactionType.HasValue)
            {
                query = query.Where(t => t.TransactionType == queryDto.TransactionType);
            }

            if (!string.IsNullOrWhiteSpace(queryDto.AssetSymbol))
            {
                query = query.Where(t => t.Asset.Symbol.Contains(queryDto.AssetSymbol));
            }

            query = queryDto.SortDescending 
                ? query.OrderByDescending(t => t.CreatedAt) 
                : query.OrderBy(t => t.CreatedAt);

            var projectedQuery = query.Select(t => t.ToTransactionDto());


            return await PagedList<TransactionDto>.CreateAsync(projectedQuery, queryDto.Page, queryDto.PageSize);
        }

















   






  }

    
}