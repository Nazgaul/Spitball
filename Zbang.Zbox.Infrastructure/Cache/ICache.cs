using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface ICache
    {
        //bool AddToCache(string key, object value, TimeSpan expiration, string region, List<string> tags);
        //bool AddToCache(string key, object value, TimeSpan expiration, string region);
        //bool RemoveFromCache(string region, List<string> tags);
        //object GetFromCache(string key, string region);



        Task<bool> AddToCacheAsync<T>(string key, T value, TimeSpan expiration, string region) where T : class;
        bool AddToCache<T>(string key, T value, TimeSpan expiration, string region) where T : class;
        bool RemoveFromCache(string region);
        Task<T> GetFromCacheAsync<T>(string key, string region) where T : class;
        T GetFromCache<T>(string key, string region) where T : class;
    }

    
}
