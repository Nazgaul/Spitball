﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search
{
    [UsedImplicitly]
    public class BingSearch : ISearch
    {
        private const string SubscriptionKey = "285e26627c874d28be01859b4fb08a58";
        private const int PageSize = 50;
        private readonly IRestClient _restClient;
        private readonly IMapper _mapper;

        public BingSearch(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        [Cache(TimeConst.Day, "bing", false)]
        [BuildLocalUrl(null, PageSize, "page")]
        [Shuffle]
        public async Task<IEnumerable<SearchResult>> DoSearchAsync(SearchModel model, int page, BingTextFormat format, CancellationToken token)
        {
            //https://docs.microsoft.com/en-us/rest/api/cognitiveservices/bing-custom-search-api-v7-reference#query-parameters
            if (model == null) throw new ArgumentNullException(nameof(model));

            var query = BuildQuery(model.UniversitySynonym, model.Courses, model.Query, model.DocType,
                model.DefaultTerm);
            var sourceQuery = BuildSources(model.Sources);

            var nvc = new NameValueCollection
            {
                ["count"] = PageSize.ToString(),
                ["customConfig"] = model.Key.Key,
                ["offset"] = (page * PageSize).ToString(),
                ["q"] = $"{query} {sourceQuery}"
            };
            //if (format != BingTextFormat.None)
            //{
            //    nvc.Add("textFormat", format.GetDescription());
            //    nvc.Add("textDecorations", bool.TrueString);
            //}
            var uri = new Uri("https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search");

            var response = await _restClient.GetAsync<BingCustomSearchResponse>(uri, nvc, new[]
            {
                new KeyValuePair<string,string>("Ocp-Apim-Subscription-Key", SubscriptionKey)
            }, token).ConfigureAwait(false);
            if (response?.WebPages?.Value == null)
            {
                return null;
            }

            return _mapper.Map<IEnumerable<WebPage>, IEnumerable<SearchResult>>(
                response.WebPages?.Value, a =>
                {
                    if (format == BingTextFormat.Html)
                    {
                        a.Items["query"] = model.Query;
                    }
                });
        }

        private static string BuildSources(IEnumerable<string> sources)
        {
            if (sources == null)
            {
                return string.Empty;
            }

            var sourceStr = sources.Select(s =>
            {
                if (string.Equals(s, "Cloudents.com", StringComparison.InvariantCultureIgnoreCase))
                {
                    s = "Spitball.co";
                }
                return $"site:{s}";
            });
            return $"({string.Join(" OR ", sourceStr)})";
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

            return string.Join(" AND ", query.Select(s => s));
        }

        public class BingCustomSearchResponse
        {
            public WebPages WebPages { get; set; }
        }

        public class WebPages
        {
            public WebPage[] Value { get; set; }
        }

        public class WebPage
        {
            //public string id { get; set; }
            //public string urlPingSuffix { get; set; }
            //public bool isFamilyFriendly { get; set; }
            //public string displayUrl { get; set; }
            //public DateTime dateLastCrawled { get; set; }
            //public bool fixedPosition { get; set; }

            public string Name { get; set; }
            public string Url { get; set; }
            public string Snippet { get; set; }
            public OpenGraphImage OpenGraphImage { get; set; }
        }





        public class OpenGraphImage
        {
            public string ContentUrl { get; set; }
        }
    }
}
