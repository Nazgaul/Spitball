
namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public class SearchItemQuery: SearchQueryBase
    {
        public SearchItemQuery(long Id, string searchText, int pageNumber)
            : base(Id, searchText, pageNumber)
        {
        }
        public override string QueryName
        {
            get { return "SearchItem"; }
        }
    }
}
