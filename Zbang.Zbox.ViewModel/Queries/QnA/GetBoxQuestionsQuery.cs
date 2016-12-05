using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.QnA
{
    public class GetBoxQuestionsQuery : IPagedQuery2, IQueryCache
    {
        public GetBoxQuestionsQuery(long boxId, int top = int.MaxValue, int skip = 0)
        {
            Top = top;
            Skip = skip;
            BoxId = boxId;
            
        }

        public static GetBoxQuestionsQuery GetBoxQueryOldVersion(long boxId, int pageNumber, int rowsPerPage)
        {
            return new GetBoxQuestionsQuery(boxId, rowsPerPage, pageNumber * rowsPerPage);
        }

        public long BoxId { get; }


        public int Top { get; }

        public int Skip { get; }

        public const int TopOfReplies = 4;
        public string CacheKey => $"{Top}_{Skip}";
        public string CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
        public TimeSpan Expiration => TimeSpan.FromMinutes(30);
    }
}
