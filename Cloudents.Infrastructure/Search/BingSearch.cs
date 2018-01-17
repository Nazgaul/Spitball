using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search
{
    public class BingSearch : ISearch
    {
        private const string SubscriptionKey = "285e26627c874d28be01859b4fb08a58";
        private readonly IRestClient _restClient;
        private readonly IKeyGenerator _keyGenerator;

        public BingSearch(IRestClient restClient, IKeyGenerator keyGenerator)
        {
            _restClient = restClient;
            _keyGenerator = keyGenerator;
        }

        [Cache(TimeConst.Day, "bing")]
        public async Task<IEnumerable<SearchResult>> DoSearchAsync(SearchModel model, BingTextFormat format, CancellationToken token)
        {
            //https://docs.microsoft.com/en-us/rest/api/cognitiveservices/bing-custom-search-api-v7-reference#query-parameters
            if (model == null) throw new ArgumentNullException(nameof(model));

            var query = BuildQuery(model.UniversitySynonym, model.Courses, model.Query, model.DocType,
                model.DefaultTerm);
            var sourceQuery = BuildSources(model.Sources);

            //TODO: need to do sort
            var nvc = new NameValueCollection
            {
                ["count"] = 50.ToString(),
                ["customConfig"] = model.Key.Key,
                ["offset"] = (model.Page * 50).ToString(),
                ["q"] = $"{query} {sourceQuery}",
                ["textFormat"] = format.GetDescription(),
                ["textDecorations"] = bool.TrueString
            };
            var uri = new Uri("https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search");

            var result = await _restClient.GetAsync(uri, nvc, new[]
            {
                new KeyValuePair<string,string>("Ocp-Apim-Subscription-Key", SubscriptionKey)
            }, token).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<BingCustomSearchResponse>(result);
            //var dictionaryOfHost = new ConcurrentDictionary<string, int>();
            var searchResult = response.WebPages?.Value?.Select((s, i) =>
          {
              Uri.TryCreate(s.OpenGraphImage?.ContentUrl, UriKind.Absolute, out var image);
              var url = new Uri(s.Url);
              return new SearchResult
              {
                  Url = url,
                  Id = _keyGenerator.GenerateKey(s.Url),
                  Image = image,
                  Snippet = s.Snippet,
                  Source = url.Host,
                  Title = s.Name,

              };
          });
            return Shuffle<SearchResult>.DoShuffle(searchResult);
        }

        public static string BuildSources(IEnumerable<string> sources)
        {
            if (sources == null)
            {
                return string.Empty;
            }

            return $"({string.Join(" OR ", sources.Select(s => $"site:{s}"))})";
        }

        public static string BuildQuery(IEnumerable<string> universitySynonym,
            IEnumerable<string> courses,
            IEnumerable<string> terms,
            string docType,
            string defaultTerm)
        {
            var query = new List<string>();
            if (defaultTerm == null) throw new ArgumentNullException(nameof(defaultTerm));
            if (universitySynonym != null)
            {
                query.Add($"({string.Join(" OR ", universitySynonym.Select(s => '"' + s + '"'))})");
            }
            if (courses != null)
            {
                query.Add(string.Join(" AND ", courses.Select(s => $"({s})")));
            }
            if (terms != null)
            {
                query.AddNotNull(string.Join(" AND ", terms.Where(w => !string.IsNullOrWhiteSpace(w)).Select(s => $"({s})")));
            }

            if (!string.IsNullOrEmpty(docType))
            {
                query.Add($"({docType})");
            }

            if (query.Count == 0)
            {
                return defaultTerm;
            }

            return string.Join(" AND ", query.Select(s => $"{s}"));
        }

        public class BingCustomSearchResponse
        {
            public string Type { get; set; }
            public WebPages WebPages { get; set; }
        }

        public class WebPages
        {
            public string WebSearchUrl { get; set; }
            public int TotalEstimatedMatches { get; set; }
            public WebPage[] Value { get; set; }
        }

        public class WebPage
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string DisplayUrl { get; set; }
            public string Snippet { get; set; }
            public DateTime DateLastCrawled { get; set; }
            public string CachedPageUrl { get; set; }
            public OpenGraphImage OpenGraphImage { get; set; }
        }

        public class OpenGraphImage
        {
            public string ContentUrl { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
