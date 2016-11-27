using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Dashboard
{
    public class LeaderBoardQuery : IQueryCache
    {
        public LeaderBoardQuery(long universityId)
        {
            UniversityId = universityId;
        }

        public long UniversityId { get; }


        public string CacheKey => UniversityId.ToString(CultureInfo.InvariantCulture);

        public string CacheRegion => "leaderboardDashboard";

        public TimeSpan Expiration => TimeSpan.FromDays(1);
    }

    public class FlashcardLeaderboardQuery
    {
        public FlashcardLeaderboardQuery(long userId, long universityId)
        {
            UserId = userId;
            UniversityId = universityId;
        }

        public long UserId { get; private set; }
        public long UniversityId { get; private set; }
    }
}
