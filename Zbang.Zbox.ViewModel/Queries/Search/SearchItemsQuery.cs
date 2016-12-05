namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchItemsQuery : SearchQuery
    {
        public SearchItemsQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50)
            : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey => "items " + GetUniversityId();
    }
}