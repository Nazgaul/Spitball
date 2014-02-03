

namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchLibraryQuery : SearchQueryBase
    {
        public SearchLibraryQuery(long universityId, string searchText, int pageNumber, long userId)
            : base(userId, searchText, pageNumber)
        {
            UniversityId = universityId;
        }
        public override string QueryName
        {
            get { return "SearchLibrary"; }
        }

        public long UniversityId { get; private set; }
    }
}
