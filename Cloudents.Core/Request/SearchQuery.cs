using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Request
{
    public class SearchQuery
    {
        public SearchQuery(IEnumerable<string> query, int page)
            : this(query, default, null, null, page, SearchCseRequestSort.None)
        {
        }

        public SearchQuery(IEnumerable<string> query, long? university,
            IEnumerable<long> courses, IEnumerable<string> source, int page, SearchCseRequestSort sort)
        {
            Query = query;
            University = university;
            Courses = courses;
            Source = source;
            Page = page;
            Sort = sort;
        }

        public IEnumerable<string> Source { get; }
        public long? University { get; }
        public IEnumerable<long> Courses { get; }
        public IEnumerable<string> Query { get; }
        public int Page { get; }
        public SearchCseRequestSort Sort { get; }
    }
}