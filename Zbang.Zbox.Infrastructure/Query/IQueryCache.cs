using System;
using Zbang.Zbox.Infrastructure.Cache;

namespace Zbang.Zbox.Infrastructure.Query
{
    public interface IQueryCache
    {
        string CacheKey { get; }
        CacheRegions CacheRegion { get; }
        TimeSpan Expiration { get; }
    }
}
