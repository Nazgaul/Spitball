using Zbang.Zbox.Infrastructure.Cache;

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchQueryMobile : SearchQuery
    {
        public SearchQueryMobile(string term,
            long userId,
            long universityId, int pageNumber = 0, int rowsPerPage = 50) :
                base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey => GetUniversityId();

        public override CacheRegions CacheRegion => CacheRegions.SearchMobile;
    }

    public class SearchItemInBox
    {
        public SearchItemInBox(string term, long boxId, int pageNumber, int rowsPerPage)
        {
            RowsPerPage = rowsPerPage;
            PageNumber = pageNumber;
            BoxId = boxId;
            Term = term;
        }

        public string Term { get; private set; }
        public int PageNumber { get; private set; }

        public int RowsPerPage { get; private set; }
        public long BoxId { get; private set; }
    }

    //public class SearchContent
    //{
    //    public string Term { get; set; }

    //}
}