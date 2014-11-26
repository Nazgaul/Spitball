using System;
using System.Collections.Generic;
namespace Zbang.Zbox.Infrastructure.Query
{
    public interface IQueryCache
    {
        string CacheKey { get; }
        string CacheRegion { get; }
        TimeSpan Expiration { get; }
    }
}
