using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetUniversityByPrefixQuery : QueryPrefixBase, IQueryCache
    {
        public GetUniversityByPrefixQuery(long userid, int pageNumber, string prefix, string country)
            :base(prefix,userid,pageNumber)
        {
            Country = country;
        }

        public string Country { get; set; }

        public string CacheKey
        {
            get { return string.Format("{0}_{1}_{2}", Country, Prefix, PageNumber); }
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
