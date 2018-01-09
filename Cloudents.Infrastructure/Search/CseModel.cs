using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Search
{
    public class SearchModel
    {
        public SearchModel(IEnumerable<string> query, IEnumerable<string> source,
            int page, SearchRequestSort sort, 
            CustomApiKey key)
        {
            Query = string.Join(" ", query);
            if (source != null)
            {
                Source = string.Join(" OR ", source.Select(s => $"site:{s}"));
            }
            Page = page;
            Sort = sort;
            Key = key;
        }

        public string Query { get; }
        public string Source { get; }
        public int Page { get; }
        public SearchRequestSort Sort { get; }
        public CustomApiKey Key { get; }
    }
}