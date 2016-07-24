using Zbang.Zbox.Infrastructure.Cache;
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

        public string CacheKey => UniversityId.ToString();

        public string CacheRegion => CacheRegions.UniversityRegion;

        public System.TimeSpan Expiration => System.TimeSpan.FromHours(2);
    }
}
