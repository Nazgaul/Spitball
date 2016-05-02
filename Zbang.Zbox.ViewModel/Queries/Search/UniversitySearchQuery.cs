
namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class UniversitySearchQuery : IPagedQuery
    {


        public UniversitySearchQuery(string term, int rowsPerPage = 50, int pageNumber = 0, string country = "")
        {
            Term = term;
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            Country = country;
        }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public string Term { get; }

        public string Country { get; }


        public override string ToString()
        {
            return string.Format("term = {0}, rowsperPage = {1} page = {2} country = {3}", Term, RowsPerPage, PageNumber, Country);

        }
    }


    //TODO: remove this
    public class UniversityByIpQuery : IPagedQuery
    {
        public UniversityByIpQuery(long ipAddress, int rowsPerPage, int pageNumber)
        {
            PageNumber = pageNumber;
            RowsPerPage = rowsPerPage;
            IpAddress = ipAddress;
        }

        public int PageNumber { get; }

        public int RowsPerPage { get; }

        public long IpAddress { get; private set; }
    }
}
