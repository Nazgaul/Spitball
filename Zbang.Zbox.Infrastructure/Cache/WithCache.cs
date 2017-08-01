using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;
using Zbang.Zbox.Infrastructure.Query;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public class WithCache : IWithCache
    {
        private readonly ICache m_Cache;
        private readonly ILogger m_Logger;
        public WithCache(ICache cache, ILogger logger)
        {
            m_Cache = cache;
            m_Logger = logger;
        }

        public async Task<TD> QueryAsync<TQ, TD>(Func<TQ, Task<TD>> getItemCallbackAsync, TQ queryParam)
            where TD : class
            where TQ : IQueryCache
        {
            var cacheKey = queryParam.CacheKey;

            var item = await m_Cache.GetFromCacheAsync<TD>(queryParam.CacheRegion,cacheKey).ConfigureAwait(false);

            if (item != default(TD)) return item;

            item = await getItemCallbackAsync(queryParam).ConfigureAwait(false);
            try
            {
                await m_Cache.AddToCacheAsync(queryParam.CacheRegion,cacheKey, item, queryParam.Expiration).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
            }
            return item;
        }

        public Task RemoveAsync<TC>(TC command)
            where TC : ICommandCache
        {
            return m_Cache.RemoveFromCacheAsync(command.CacheRegion);
        }
    }
}
