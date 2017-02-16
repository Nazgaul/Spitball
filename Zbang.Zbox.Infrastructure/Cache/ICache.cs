using System;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface ICache
    {
        Task AddToCacheAsync<T>(string region, string key, T value, TimeSpan expiration) where T : class;
        void AddToCache<T>(string region, string key, T value, TimeSpan expiration) where T : class;
        Task RemoveFromCacheAsync(string region);
        Task<T> GetFromCacheAsync<T>(string region, string key) where T : class;

        Task RemoveFromCacheAsyncSlowAsync(string region);
        T GetFromCache<T>(string region, string key) where T : class;
    }


}
