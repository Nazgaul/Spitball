namespace Zbang.Zbox.ViewModel.Queries.Jared
{
    public class JaredSearchQuery
    {
        public string Name { get; set; }
        public int PageNumber { get; set; }
        public bool IsReviewed { get; set; }
        public bool IsSearchType { get; set; }
    }
    public class SearchTermQuery
    {
        public SearchTermQuery(string term)
        {
            Term = term;
        }
        public string Term { get; set; }
    }
}
