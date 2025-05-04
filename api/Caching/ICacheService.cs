using api.Dtos.Comment;
using api.Dtos.AssetDtos;
using api.Models;

namespace api.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveAsync(string key);
    }
}