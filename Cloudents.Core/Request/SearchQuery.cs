using System.Collections.Generic;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Request
{
    public class SearchQuery
    {
        public SearchQuery(IEnumerable<string> query, int page)
            : this(query, null, null, null, page, SearchCseRequestSort.None)
        {
        }

        public SearchQuery(IEnumerable<string> query, IEnumerable<string> universitySynonym,
            IEnumerable<string> course, IEnumerable<string> source, int page, SearchCseRequestSort sort)
        {
            Query = query;
            UniversitySynonym = universitySynonym;
            Course = course;
            Source = source;
            Page = page;
            Sort = sort;
        }

        public IEnumerable<string> Source { get; }
        public IEnumerable<string> UniversitySynonym { get; }
        public IEnumerable<string> Course { get; }
        public IEnumerable<string> Query { get; }
        public int Page { get; }
        public SearchCseRequestSort Sort { get; }
    }
}