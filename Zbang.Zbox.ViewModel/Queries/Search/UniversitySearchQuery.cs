
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

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

        public string Term { get; private set; }

        public string Country { get; private set; }


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

        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }

        public long IpAddress { get; private set; }
    }
}
