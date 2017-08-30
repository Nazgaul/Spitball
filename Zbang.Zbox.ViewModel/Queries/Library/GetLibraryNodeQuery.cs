using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Library
{
    public class GetLibraryNodeQuery : QueryBase , IQueryCache
    {
        public GetLibraryNodeQuery(long universityId, Guid? parentNode, long userId)
            : base(userId)
        {
            UniversityId = universityId;
            ParentNode = parentNode;
        }
        public long UniversityId { get; }
        public Guid? ParentNode { get; }

        public string CacheKey => $"{ParentNode}";
        public CacheRegions CacheRegion => CacheRegions.BuildNodesRegion(UniversityId);
        public TimeSpan Expiration  => TimeSpan.FromHours(1);
    }
}
