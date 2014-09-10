using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.User
{
    public class GetDashboardQuery :  IQueryCache
    {
        public GetDashboardQuery(long universityId)
        {
            UniversityId = universityId;
        }
        public long UniversityId { get; private set; }

        public string CacheKey
        {
            get { return UniversityId.ToString(CultureInfo.InvariantCulture); }
        }

        public string CacheRegion
        {
            get { return "GetDashboardQuery"; }
        }

        public System.Collections.Generic.List<string> CacheTags
        {
            get { return null; }
        }

        public System.TimeSpan Expiration
        {
            get { return System.TimeSpan.FromMinutes(1); }
        }
    }
}
