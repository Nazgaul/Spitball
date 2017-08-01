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

        public CacheRegions CacheRegion => CacheRegions.University;

        public System.TimeSpan Expiration => System.TimeSpan.FromHours(6);
    }
}
