using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Cache;
using Cloudents.Infrastructure.Search.Query;
using Google;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace Cloudents.Infrastructure.Search
{
    public abstract class CseSearch
    {
        public IKeyGenerator KeyGenerator { get; set; }
        public ICacheProvider<IEnumerable<SearchResult>> CacheProvider { get; set; }

        protected async Task<IEnumerable<SearchResult>> DoSearchAsync(GoogleQuery query,
            CancellationToken token)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

           // var cacheResult = CacheProvider.Get(query, CacheRegion.SearchCse);
           // if (cacheResult != null)
           // {
           //     return cacheResult;
           // }
            var initializer = new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCZEbkX9Of6pZQ47OD0VA9a8fd1A6IvW6E",

            };
            var p = new CustomsearchService(initializer);
            var request = new CseResource.ListRequest(p, string.Join(" ", query))
            {
                Start = query.Page == 0 ? 1 : (query.Page * 10) + 1,
                SiteSearch = query.Source,
                Cx = query.Key.Key,
                Fields = "items(title,link,snippet,pagemap/cse_image,displayLink)",
                Sort = query.Sort == SearchRequestSort.Date ? "date" : string.Empty
            };
            try
            {
                var result = await request.ExecuteAsync(token).ConfigureAwait(false);
                var retVal =  result.Items?.Select(s =>
                {
                    string image = null;
                    if (s.Pagemap != null && s.Pagemap.TryGetValue("cse_image", out var value)
                        && value[0].TryGetValue("src", out var t))
                    {
                        image = t.ToString();
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
                }).ToList();
               // CacheProvider.Set(query, CacheRegion.SearchCse, retVal, TimeSpan.FromDays(1));
                return retVal;
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return null;
            }
            catch (GoogleApiException ex)
            {
                ex.Data.Add("params", new
                {
                    query,
                    query.Source,
                    query.Page,
                    SearchRequestSort = query.Sort,
                    query.Key
                });
                throw;
            }
        }
    }
}
