using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Google;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace Cloudents.Infrastructure.Search
{
    public abstract class Base
    {
        public IKeyGenerator KeyGenerator { get; set; }

        protected async Task<IEnumerable<SearchResult>> DoSearchAsync(string query,
            string source,
            int page,
            SearchRequestSort sort,
            CustomApiKey key,
            CancellationToken token)
        {
            var initializer = new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCZEbkX9Of6pZQ47OD0VA9a8fd1A6IvW6E",

            };
            var p = new CustomsearchService(initializer);
            var request = new CseResource.ListRequest(p, string.Join(" ", query))
            {
                Start = page == 0 ? 1 : (page * 10) + 1,
                SiteSearch = source,
                Cx = key.Key,
                Fields = "items(title,link,snippet,pagemap/cse_image,displayLink)",
                Sort = sort == SearchRequestSort.Date ? "date" : string.Empty
            };
            try
            {
                var result = await request.ExecuteAsync(token).ConfigureAwait(false);

                return result.Items?.Select(s =>
                {
                    string image = null;
                    if (s.Pagemap != null && s.Pagemap.TryGetValue("cse_image", out var value))
                    {
                        if (value[0].TryGetValue("src", out var t))
                        {
                            image = t.ToString();
                        }
                    }
                    return new SearchResult

                    {
                        Id = KeyGenerator.GenerateKey(s.Link),
                        Url = s.Link,
                        Title = s.Title,
                        Snippet = s.Snippet,
                        Image = image,
                        Source = s.DisplayLink

                    };
                });
            }
            catch (GoogleApiException ex)
            {
                ex.Data.Add("params", new
                {
                    query,
                    source,
                    page,
                    sort,
                    key
                });
                throw;
            }
        }
    }
}
