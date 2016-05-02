using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Dashboard
{
    public class UniversityQuery :  IQueryCache
    {
        public UniversityQuery(long universityId)
        {
            UniversityId = universityId;
        }
        public long UniversityId { get; }

        public string CacheKey
        {
            get { return UniversityId.ToString(CultureInfo.InvariantCulture); }
        }

        public string CacheRegion
        {
            get { return "GetDashboardQuery"; }
        }

        public System.TimeSpan Expiration
        {
            get { return System.TimeSpan.FromHours(2); }
        }
    }
}
