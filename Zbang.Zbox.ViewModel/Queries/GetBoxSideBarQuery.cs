using System;
using System.Collections.Generic;
using System.Globalization;

using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetBoxSideBarQuery :IQueryCache
    {
        public GetBoxSideBarQuery(long boxId)
        {
            BoxId = boxId;
        }

        public long BoxId { get; private set; }
        public string CacheKey
        {
            get { return BoxId.ToString(CultureInfo.InvariantCulture); }
        }

        public string CacheRegion
        {
            get { return "BoxRecommendedCourses"; }
        }

        public List<string> CacheTags
        {
            get { return new List<string>(); }
        }

        public TimeSpan Expiration
        {
            get { return TimeSpan.FromHours(1); }
        }
    }
}
