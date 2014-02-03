using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public interface ICache
    {
        bool AddToCache(string key, object value, TimeSpan experation, string region, List<string> tags);
        bool AddToCache(string key, object value, TimeSpan experation, string region);
        bool RemoveFromCache(string region, List<string> tags);
        object GetFromCache(string key, string region);
      
    }

    
}
