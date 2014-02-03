
namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchBoxQuery : SearchQueryBase
    {
        public SearchBoxQuery(long userId, string searchText, int pageNumber)
            : base(userId, searchText, pageNumber)
        { }
        public override string QueryName
        {
            get { return "SearchBox"; }
        }
    }
}
