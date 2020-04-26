namespace Cloudents.Core.Query
{
    public class TutorListTabSearchQuery
    {
        public TutorListTabSearchQuery(string term, string? country, int page, int pageSize = 25)
        {
            Term = term;
            Page = page;
            Country = country;
            PageSize = pageSize;
        }

        public string Term { get; }
        public int Page { get; }
        public int PageSize { get; }

        public string? Country { get; }
    }
}