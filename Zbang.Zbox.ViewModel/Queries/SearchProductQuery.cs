namespace Zbang.Zbox.ViewModel.Queries
{
    public class SearchProductQuery
    {
        public SearchProductQuery(string term)
        {
            Term = term;
        }
        public string Term { get; private set; }
    }
}
