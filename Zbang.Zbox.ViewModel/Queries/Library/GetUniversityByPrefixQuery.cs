using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetUniversityByPrefixQuery :  IQueryCache
    {
        public string CacheKey
        {
            get { return "libraryChoose"; }
        }

        public string CacheRegion
        {
            get { return "libraryChoose"; }
        }

        public List<string> CacheTags
        {
            get { return null; }
        }

        public TimeSpan Expiration
        {
            get { return TimeSpan.FromHours(1); }
        }
    }
}
