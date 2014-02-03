
namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public abstract class SearchQueryBase : QueryPagedBase
    {
        public SearchQueryBase(long Id, string searchText, int pageNumber)
            : base(Id, pageNumber)
        {
            this.SearchText = searchText.Replace(' ' , '%');
        }

        /// <summary>
        /// The name of the query we want to invoke
        /// </summary>
        public abstract string QueryName { get; } 
       
        public string SearchText { get; private set; }      

       
    }
}
