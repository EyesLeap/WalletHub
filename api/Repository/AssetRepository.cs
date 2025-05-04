using api.Data;
using api.Dtos.Currency;
using api.Dtos.AssetDtos;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class AssetRepository : IAssetRepository
    {
        private readonly ApplicationDBContext _context;
        public AssetRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Asset> AddAsync(Asset asset)
        {
            await _context.Assets.AddAsync(asset);
            await _context.SaveChangesAsync();
            return asset;
        }

        public async Task<Asset?> DeleteAsync(int portfolioId,  string symbol)
        {
            var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Portfolio.Id == portfolioId && 
            a.Symbol.ToLower() == symbol.ToLower());

            if (asset == null)
            {
                return null;
            }

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return asset;

            
        }

        public async Task<IEnumerable<GetAssetDto>> GetAllByPortfolioIdAsync(int portfolioId)
        {
            var currencies = await _context.Assets
                .Where(pc => pc.PortfolioId == portfolioId)
                .ToListAsync();

            return currencies.Select(pc => pc.ToGetAssetDto()).ToList();
        }

        public async Task<Asset> GetByPortfolioAndSymbol(int portfolioId, string symbol)
        {
            var asset = await _context.Assets
                .Where(a => a.PortfolioId == portfolioId && 
                a.Symbol.ToLower() == symbol.ToLower())
                .FirstOrDefaultAsync();

            if (asset == null) return null;

            return asset;
        }

        public async Task<Asset?> UpdateAsync(Asset asset)
        {
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();

            return asset;
        }

        


    }
}