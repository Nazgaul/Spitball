using Cloudents.Core.Enum;
using Cloudents.Core.Request;

namespace Cloudents.Infrastructure.Search.Query
{
    public class GoogleQuery : IQuery
    {
        public GoogleQuery(string query, string source, int page, SearchRequestSort sort, CustomApiKey key)
        {
            Query = query;
            Source = source;
            Page = page;
            Sort = sort;
            Key = key;
        }

       

        public string Query { get; }
        public string Source { get; }
        public int Page { get; }
        public SearchRequestSort Sort { get; }

        public CustomApiKey Key { get; }

        public string CacheKey => $"{Key}_{Query}_{Source}_{Page}_{Sort}";
    }
}