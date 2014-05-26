namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchLibraryDashBoardQuery : SearchQueryBase
    {
        public SearchLibraryDashBoardQuery(long universityId, string searchText, int pageNumber, long userId)
            : base(userId, searchText, pageNumber)
        {
            UniversityId = universityId;
        }
        public override string QueryName
        {
            get { return "SearchLibraryWithPrivate"; }
        }

        public long UniversityId { get; private set; }
    }
}
