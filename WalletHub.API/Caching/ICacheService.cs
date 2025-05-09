using WalletHub.API.Dtos.Comment;
using WalletHub.API.Dtos.AssetDtos;
using WalletHub.API.Models;

namespace WalletHub.API.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveAsync(string key);
    }
}