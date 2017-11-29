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
            string course, string source, int page, SearchCseRequestSort sort)
        {
            Query = query;
            UniversitySynonym = universitySynonym;
            Course = course;
            Source = source;
            Page = page;
            Sort = sort;
        }

        public string Source { get; }
        public IEnumerable<string> UniversitySynonym { get; }
        public string Course { get; }
        public IEnumerable<string> Query { get; }
        public int Page { get; }
        public SearchCseRequestSort Sort { get; }
    }
}