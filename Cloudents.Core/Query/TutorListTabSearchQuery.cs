namespace Cloudents.Core.Query
{
    public class TutorListTabSearchQuery
    {
        public TutorListTabSearchQuery(string term, string country, int page)
        {
            Term = term;
            Page = page;
            Country = country;
        }

        public string Term { get; }
        public int Page { get; }

        public string Country { get; }
    }
}