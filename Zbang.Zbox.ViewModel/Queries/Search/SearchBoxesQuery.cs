namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchBoxesQuery : SearchQuery
    {
        public SearchBoxesQuery(string term, long userId, long universityId, int pageNumber = 0, int rowsPerPage = 50) : base(term, userId, universityId, pageNumber, rowsPerPage)
        {
        }

        public override string CacheKey => "boxes " + GetUniversityId();
    }
}