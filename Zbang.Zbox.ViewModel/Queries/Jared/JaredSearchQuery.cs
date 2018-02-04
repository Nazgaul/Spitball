namespace Zbang.Zbox.ViewModel.Queries.Jared
{
   
    public class SearchTermQuery
    {
        public SearchTermQuery(string term)
        {
            Term = term;
        }
        public string Term { get; set; }
    }
}
