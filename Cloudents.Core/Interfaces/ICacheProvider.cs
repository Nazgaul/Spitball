using System;
using Cloudents.Core.Models;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface ICacheProvider<T>
    {
        T Get(IQuery key, CacheRegion region);
        void Set(IQuery key, CacheRegion region, T value, TimeSpan expire);
    }
}