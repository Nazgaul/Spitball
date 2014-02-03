using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetUserMinProfileQuery : IQueryCache
    {
        public GetUserMinProfileQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; private set; }

        public string CacheKey
        {
            get { return UserId.ToString(); }
        }

        public string CacheRegion
        {
            get { return "UserMinProfile"; }
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
