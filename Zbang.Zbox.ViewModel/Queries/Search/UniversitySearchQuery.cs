namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class UniversitySearchQuery : IPagedQuery
    {

        public UniversitySearchQuery(string term, int rowsPerPage = 50, int pageNumber = 0)
        {
            Term = term;
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
        }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

        public string Term { get; private set; }
    }
}
