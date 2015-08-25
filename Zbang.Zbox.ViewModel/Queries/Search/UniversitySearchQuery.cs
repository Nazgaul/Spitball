
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

        public override string ToString()
        {
            return string.Format("term = {0}, rowsperPage = {1} page = {2}", Term, RowsPerPage, PageNumber);
            
        }
    }

    public class UniversityByIpQuery : IPagedQuery
    {
        public UniversityByIpQuery(long ipAddress, int rowsPerPage, int pageNumber)
        {
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
            IpAddress = ipAddress;
        }

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

        public long IpAddress { get; private set; }
    }
}
