
using Zbang.Zbox.Infrastructure.Query;
namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetUniversityDetailQuery : QueryBase , IQueryCache
    {
        public GetUniversityDetailQuery(long universityId,long universityWrapperId)
            : base(universityId)
        {
            UniversityWrapperId = universityWrapperId;
        }

        public long UniversityWrapperId { get; private set; }

        public string CacheKey
        {
            get { return string.Format("{0}_{1}", UserId, UniversityWrapperId); }
        }

        public string CacheRegion
        {
            get { return "UniversityDetail"; }
        }

        public System.Collections.Generic.List<string> CacheTags
        {
            get { return null; }
        }

        public System.TimeSpan Expiration
        {
            get { return System.TimeSpan.FromMinutes(15); }
        }
    }
}
