namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class UserInBoxSearchQuery : IPagedQuery
    {
        public UserInBoxSearchQuery(string term, long universityId, long boxId, int pageNumber, int rowsPerPage)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
            UniversityId = universityId;
            Term = term;
        }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public string Term { get; private set; }

        public long UniversityId { get; private set; }

        public long BoxId { get; private set; }
    }
}
