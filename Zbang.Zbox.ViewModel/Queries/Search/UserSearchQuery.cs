namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class UserSearchQuery : IPagedQuery
    {
        public UserSearchQuery(string term, long universityId, int pageNumber, int rowsPerPage, long userId)
        {
            RowsPerPage = rowsPerPage;
            UserId = userId;
            PageNumber = pageNumber;
            UniversityId = universityId;
            Term = term;
        }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public string Term { get; private set; }

        public long UniversityId { get; private set; }

        public long UserId { get; private set; }
    }
}