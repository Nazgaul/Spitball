using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Google;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace Cloudents.Infrastructure.Search
{
    public interface ICseSearch
    {
        Task<IEnumerable<SearchResult>> DoSearchAsync(string query,
            string source,
            int page,
            SearchCseRequestSort sort,
            CustomApiKey key,
            CancellationToken token);
    }
    public class CseSearch : ICseSearch
    {
        private readonly IKeyGenerator m_KeyGenerator;

        public CseSearch(IKeyGenerator keyGenerator)
        {
            m_KeyGenerator = keyGenerator;
        }

        [Cache(TimeConst.Day, "cse")]
        public async Task<IEnumerable<SearchResult>> DoSearchAsync(string query,
            string source,
            int page,
            SearchCseRequestSort sort,
            CustomApiKey key,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentNullException(nameof(query));
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
                Sort = sort == SearchCseRequestSort.Date ? "date" : string.Empty
            };
            try
            {
                var result = await request.ExecuteAsync(token).ConfigureAwait(false);
                return result.Items?.Select(s =>
                {
                    string image = null;
                    if (s.Pagemap != null && s.Pagemap.TryGetValue("cse_image", out var value)
                        && value[0].TryGetValue("src", out var t))
                    {
                        image = t.ToString();
                    }
                    return new SearchResult
                    {
                        Id = m_KeyGenerator.GenerateKey(s.Link),
                        Url = s.Link,
                        Title = s.Title,
                        Snippet = s.Snippet,
                        Image = image,
                        Source = s.DisplayLink

                    };
                });
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return null;
            }
            catch (GoogleApiException ex)
            {
                ex.Data.Add("q", query);
                ex.Data.Add("source", source);
                ex.Data.Add("page", page);
                ex.Data.Add("sort", sort);
                ex.Data.Add("key", key);
                throw;
            }
        }
    }
}
