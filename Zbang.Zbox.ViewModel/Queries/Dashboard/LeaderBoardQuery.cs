namespace Zbang.Zbox.ViewModel.Queries.Dashboard
{
    public class LeaderboardQuery : IPagedQuery
    {
        public LeaderboardQuery(long userId,  int pageNumber = 0)
        {
            UserId = userId;
            PageNumber = pageNumber;
            RowsPerPage = 1000;
        }

        public long UserId { get; private set; }

        public int PageNumber { get; }
        public int RowsPerPage { get; }
    }
}
