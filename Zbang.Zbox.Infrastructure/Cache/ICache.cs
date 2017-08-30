using System;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface ICache
    {
        Task AddToCacheAsync<T>(CacheRegions region, string key, T value, TimeSpan expiration) where T : class;
        void AddToCache<T>(CacheRegions region, string key, T value, TimeSpan expiration) where T : class;
        Task RemoveFromCacheAsync(CacheRegions region);
        Task<T> GetFromCacheAsync<T>(CacheRegions region, string key) where T : class;

        Task RemoveFromCacheAsyncSlowAsync(CacheRegions region);
        T GetFromCache<T>(CacheRegions region, string key) where T : class;
    }
}
