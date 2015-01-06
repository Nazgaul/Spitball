

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class BoxSearchQuery : IPagedQuery, IUserQuery
    {
        public BoxSearchQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
        {
            UniversityId = universityId;
            Term = term;
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            UserId = userId;
        }

        public long UserId { get; private set; }

        public long UniversityId { get; private set; }

        public int PageNumber { get; private set; }


        public int RowsPerPage { get; private set; }

        public string Term { get; private set; }

    }
}
