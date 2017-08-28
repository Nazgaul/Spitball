
using System;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Boxes
{
    public class GetUserWithFriendQuery : IPagedQuery
    {
        public GetUserWithFriendQuery(long friendId, int pageNumber = 0, int rowsPerPage = int.MaxValue)
        {
            FriendId = friendId;
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
        }
        public long FriendId { get; private set; }

        public int PageNumber
        {
            get;
        }

        public int RowsPerPage
        {
            get;
        }

    }

    public class GetUserStatsQuery : IQueryCache
    {
        public GetUserStatsQuery(long friendId)
        {
            FriendId = friendId;
        }
        public long FriendId { get; private set; }

        public string CacheKey => $"friendId{FriendId}";
        public CacheRegions CacheRegion => CacheRegions.Profile;
        public TimeSpan Expiration => TimeSpan.FromDays(1);
    }
}
