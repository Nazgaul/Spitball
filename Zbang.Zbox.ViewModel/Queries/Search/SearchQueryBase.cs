
namespace Zbang.Zbox.ViewModel.Queries.Search
{
    public abstract class SearchQueryBase : QueryPagedBase
    {
        protected SearchQueryBase(long id, string searchText, int pageNumber)
            : base(id, pageNumber)
        {
            SearchText = searchText.Replace(' ' , '%');
        }

        /// <summary>
        /// The name of the query we want to invoke
        /// </summary>
        public abstract string QueryName { get; } 
       
        public string SearchText { get; private set; }      

       
    }
}
