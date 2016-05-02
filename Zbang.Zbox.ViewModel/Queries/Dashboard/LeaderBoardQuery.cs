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


        public string CacheKey
        {
            get { return UniversityId.ToString(CultureInfo.InvariantCulture); }
        }

        public string CacheRegion
        {
            get { return "leaderboardDashboard"; }
        }

        public TimeSpan Expiration
        {
            get { return TimeSpan.FromDays(1); }
        }
    }
}
