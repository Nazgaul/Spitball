﻿using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Query;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public class WithCache : IWithCache
    {
        readonly ICache m_Cache;
        public WithCache(ICache cache)
        {
            m_Cache = cache;
        }


        public TD Query<TQ, TD>(Func<TQ, TD> getItemCallback, TQ queryParam)
            where TD : class
            where TQ : IQueryCache
        {
            if (getItemCallback == null) throw new ArgumentNullException("getItemCallback");
            string cacheKey = queryParam.CacheKey;

            var item = m_Cache.GetFromCache<TD>(cacheKey, queryParam.CacheRegion);
           
            if (item != null) return item;
            item = getItemCallback(queryParam);
            m_Cache.AddToCache(cacheKey, item, queryParam.Expiration, queryParam.CacheRegion);
            return item;
        }

        public async Task<TD> QueryAsync<TQ, TD>(Func<TQ, Task<TD>> getItemCallbackAsync, TQ queryParam)
            where TD : class
            where TQ : IQueryCache
        {
            string cacheKey = queryParam.CacheKey;

            var item = await m_Cache.GetFromCacheAsync<TD>(cacheKey, queryParam.CacheRegion);
            
            if (item != default(TD)) return item;

            item = await getItemCallbackAsync(queryParam);
            try
            {
                await m_Cache.AddToCacheAsync(cacheKey, item, queryParam.Expiration, queryParam.CacheRegion);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
            }
            return item;
        }

        //public TCr Command<TC, TCr>(Func<TC, TCr> invokeFunction, TC command)
        //    where TCr : ICommandResult
        //    where TC : ICommandCache
        //{
        //    if (invokeFunction == null) throw new ArgumentNullException("invokeFunction");

        //    TCr retVal = invokeFunction(command);
        //    m_Cache.RemoveFromCache(command.CacheRegion, command.CacheTags);
        //    return retVal;
        //}

        //public void Command<TC>(Action<TC> invokeFunction, TC command)
        //    where TC : ICommandCache
        //{
        //    if (invokeFunction == null) throw new ArgumentNullException("invokeFunction");
        //    invokeFunction(command);
        //    m_Cache.RemoveFromCache(command.CacheRegion, command.CacheTags);
        //}
    }
}
