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
        public WithCache(ICache cache)
        {
            m_Cache = cache;
        }

        public async Task<TD> QueryAsync<TQ, TD>(Func<TQ, Task<TD>> getItemCallbackAsync, TQ queryParam)
            where TD : class
            where TQ : IQueryCache
        {
            var cacheKey = queryParam.CacheKey;

            var item = await m_Cache.GetFromCacheAsync<TD>(queryParam.CacheRegion,cacheKey);

            if (item != default(TD)) return item;

            item = await getItemCallbackAsync(queryParam);
            try
            {
                await m_Cache.AddToCacheAsync(queryParam.CacheRegion,cacheKey, item, queryParam.Expiration);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(ex);
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
