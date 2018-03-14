using System.Collections.Generic;

namespace Cloudents.Core.Request
{
    public class SearchQuery
    {
        public static SearchQuery Document(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> sources, int page, string docType)
        {
            return new SearchQuery(query, university, courses, sources, page,  docType);
        }

        public static SearchQuery Flashcard(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> sources, int page)
        {
            return new SearchQuery(query, university, courses, sources, page, null);
        }

        public static SearchQuery Ask(IEnumerable<string> query, int page, IEnumerable<string> sources)
        {
            return new SearchQuery(query, null, null, sources, page,  null);
        }

        private SearchQuery(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> source, int page, string docType)
        {
            Query = query;

            University = university;
            Courses = courses;
            Source = source;
            Page = page;
           // Sort = sort;
            DocType = docType;
        }

        public IEnumerable<string> Source { get; }
        public long? University { get; }
        public IEnumerable<long> Courses { get; }
        public IEnumerable<string> Query { get; }
        public int Page { get; }
       // public SearchRequestSort Sort { get; }

        public string DocType { get; }
    }
}