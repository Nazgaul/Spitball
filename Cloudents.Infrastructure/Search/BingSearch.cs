using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Converters;
using JetBrains.Annotations;
using IMapper = AutoMapper.IMapper;

namespace Cloudents.Infrastructure.Search
{
    /// <summary>
    /// <remarks>https://docs.microsoft.com/en-us/rest/api/cognitiveservices/bing-custom-search-api-v7-reference#query-parameters</remarks>
    /// </summary>
    [UsedImplicitly]
    public class BingSearch : ISearch
    {
        private const string SubscriptionKey = "285e26627c874d28be01859b4fb08a58";
        private const int PageSize = 50;
        private readonly IRestClient _restClient;
        private readonly IMapper _mapper;
        private readonly IShuffle _shuffle;

        public BingSearch(IRestClient restClient, IMapper mapper, IShuffle shuffle)
        {
            _restClient = restClient;
            _mapper = mapper;
            _shuffle = shuffle;
        }

        [Cache(TimeConst.Day, "bing3", false)]
        [BuildLocalUrl(null, PageSize, "page")]
        public async Task<IEnumerable<SearchResult>> SearchAsync(SearchModel model,
            int page, HighlightTextFormat format, CancellationToken token)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var query = BuildQuery(model.UniversitySynonym, model.Courses, model.Query,
                model.Key.DefaultPhrase);
            var sourceQuery = BuildSources(model.Sources);

            var offset = page * PageSize;
            var nvc = new NameValueCollection
            {
                ["count"] = PageSize.ToString(),
                ["customConfig"] = model.Key.Key,
                ["offset"] = offset.ToString(),
                ["q"] = $"{query} {sourceQuery}"
            };
            var uri = new Uri("https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search");

            var response = await _restClient.GetAsync<BingCustomSearchResponse>(uri, nvc, new[]
            {
                new KeyValuePair<string,string>("Ocp-Apim-Subscription-Key", SubscriptionKey)
            }, token).ConfigureAwait(false);
            if (response?.WebPages?.Value == null)
            {
                return null;
            }

            if (response.WebPages.TotalEstimatedMatches < offset)
            {
                return null;
            }

            var retVal = _mapper.Map<IEnumerable<BingWebPage>, IEnumerable<SearchResult>>(response.WebPages.Value, a =>
             {
                 if (format == HighlightTextFormat.Html)
                 {
                     a.Items[BingConverter.KeyTermHighlight] = model.Query;
                 }

                 a.Items[BingConverter.KeyPriority] = model.Key.Priority;
             });
            retVal = _shuffle.ShuffleBySource(retVal);
            return _shuffle.ShuffleByPriority(retVal);
        }

        private static string BuildSources(IEnumerable<string> sources)
        {
            if (sources == null)
            {
                return string.Empty;
            }

            var sites = string.Join(" OR ", sources.Select(s => $"site:{s}"));
            if (string.IsNullOrEmpty(sites))
            {
                return string.Empty;
            }

            return $"({sites})";
        }

        private static string BuildQuery(IEnumerable<string> universitySynonym,
            IEnumerable<string> courses,
            string term,
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
            if (!string.IsNullOrEmpty(term))
            {
                query.Add($"({term})");
                //query.AddNotNull(string.Join(" AND ", terms.Where(w => !string.IsNullOrWhiteSpace(w)).Select(s => $"({s})")));
            }

            //if (!string.IsNullOrEmpty(docType))
            //{
            //    query.Add($"({docType})");
            //}

            if (query.Count == 0)
            {
                return defaultTerm;
            }

            return string.Join(" AND ", query.Select(s => s));
        }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BingCustomSearchResponse
    {
        public BingWebPages WebPages { get; set; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BingWebPages
    {
        public BingWebPage[] Value { get; set; }
        public long TotalEstimatedMatches { get; set; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BingWebPage
    {
        public string Id { get; set; }
        /*public string urlPingSuffix { get; set; }
        public bool isFamilyFriendly { get; set; }
        public string displayUrl { get; set; }
        public DateTime dateLastCrawled { get; set; }
        public bool fixedPosition { get; set; }*/

        public string Name { get; set; }
        public string Url { get; set; }
        public string Snippet { get; set; }
        public OpenGraphImage OpenGraphImage { get; set; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class OpenGraphImage
    {
        public string ContentUrl { get; set; }
    }
}
