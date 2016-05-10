using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Cache
{
    public static class CacheRegions
    {
        public static string BuildFeedRegion(long boxId)
        {
            return $"feed_{boxId}";
        }
    }
}
