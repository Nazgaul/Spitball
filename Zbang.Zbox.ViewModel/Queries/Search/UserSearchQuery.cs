namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class UserSearchQuery : IPagedQuery
    {
        public UserSearchQuery(string term, long universityId, long boxId, int pageNumber, int rowsPerPage)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
            UniversityId = universityId;
            Term = term;
        }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }


        public string Term { get; private set; }

        public long UniversityId { get; private set; }

        public long BoxId { get; private set; }
    }
}
