using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries
{
    public class GetHomeBoxesUniversityQuery : IQueryCache
    {
        public GetHomeBoxesUniversityQuery(long? universityId, string country)
        {
            UniversityId = universityId;
            if (!string.IsNullOrEmpty(country))
            {
                Country = country;
            }
        }

        public long? UniversityId { get; }

        public string Country { get; }

        public string CacheKey => $"uni{UniversityId.GetValueOrDefault(-1)}country{Country}";
        public CacheRegions CacheRegion => CacheRegions.Homepage;
        public TimeSpan Expiration => TimeSpan.FromDays(1);
    }
}