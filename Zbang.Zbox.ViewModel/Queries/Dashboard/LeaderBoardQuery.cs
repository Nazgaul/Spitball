using System;
using System.Globalization;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ViewModel.Queries.Dashboard
{
    public class LeaderBoardQuery : IPagedQuery
    {
        public LeaderBoardQuery(long userId, bool myself, int pageNumber = 0)
        {
            UserId = userId;
            PageNumber = pageNumber;
            Myself = myself;
            RowsPerPage = 1000;
        }


        public long UserId { get; private set; }

        public bool Myself { get; private set; }

        public int PageNumber { get; }
        public int RowsPerPage { get; }
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
