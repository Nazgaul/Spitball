using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Google;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace Cloudents.Infrastructure.Search
{
    internal class CseSearch : ISearch
    {
        private readonly IKeyGenerator _keyGenerator;
        private const int NumberOfPagesPerRequest = 2;
        public CseSearch(IKeyGenerator keyGenerator)
        {
            _keyGenerator = keyGenerator;
        }

        [Cache(TimeConst.Day, "cse")]
        public async Task<IEnumerable<SearchResult>> DoSearchAsync(SearchModel model,
            CancellationToken token)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            //TODO - if we need to use this we need to do some changes.
            //if (string.IsNullOrEmpty(model.Query))
            //    throw new ArgumentNullException(nameof(model.Query));
            var initializer = new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyCZEbkX9Of6pZQ47OD0VA9a8fd1A6IvW6E",

            };
            var p = new CustomsearchService(initializer);
            var request = new CseResource.ListRequest(p, string.Join(" ", model.Query))
            {
                Start = model.Page == 0 ? 1 : (model.Page * 10) + 1,
                //SiteSearch = model.Source,
                Hq = model.Sources,
                Cx = model.Key.Key,
                Fields = "items(title,link,snippet,pagemap/cse_image,displayLink)",
                Sort = model.Sort == SearchRequestSort.Date ? "date" : string.Empty
            };
            try
            {
                var result = await request.ExecuteAsync(token).ConfigureAwait(false);
                return result.Items?.Select(s =>
                {
                    Uri image = null;
                    if (s.Pagemap != null && s.Pagemap.TryGetValue("cse_image", out var value)
                        && value[0].TryGetValue("src", out var t))
                    {
                        image = new Uri(t.ToString());
                        image = image.ChangeToHttps();
                    }
                    return new SearchResult
                    {
                        Id = _keyGenerator.GenerateKey(s.Link),
                        Url = new Uri(s.Link),
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
                ex.Data.Add("q", model.Query);
                ex.Data.Add("source", model.Sources);
                ex.Data.Add("page", model.Page);
                ex.Data.Add("sort", model.Sort);
                ex.Data.Add("key", model.Key);
                throw;
            }
        }
    }
}
