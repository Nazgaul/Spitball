namespace Zbang.Zbox.ViewModel.Queries
{
    public class SearchProductQuery
    {
        public SearchProductQuery(string term, int? universityId)
        {
            Term = term;
            UniversityId = universityId;
        }

        public string Term { get; private set; }

        public int? UniversityId { get; private set; }
    }
}
