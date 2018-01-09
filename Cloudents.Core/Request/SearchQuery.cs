using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Request
{
    public class SearchQuery
    {
        public static SearchQuery Document(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> source, int page, SearchRequestSort sort, string docType)
        {
            return new SearchQuery(query, university, courses, source, page, sort, docType);
        }

        public static SearchQuery Flashcard(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> source, int page, SearchRequestSort sort)
        {
            return new SearchQuery(query, university, courses, source, page, sort, null);
        }

        public static SearchQuery Ask(IEnumerable<string> query, int page)
        {
            return new SearchQuery(query, default, null, null, page, default, null);
        }

        private SearchQuery(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> source, int page, SearchRequestSort sort, string docType)
        {
            Query = query;

            University = university;
            Courses = courses;
            Source = source;
            Page = page;
            Sort = sort;
            DocType = docType;
        }

        public IEnumerable<string> Source { get; }
        public long? University { get; }
        public IEnumerable<long> Courses { get; }
        public IEnumerable<string> Query { get; }
        public int Page { get; }
        public SearchRequestSort Sort { get; }

        public string DocType { get; }
    }
}