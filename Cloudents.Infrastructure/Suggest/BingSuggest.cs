using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Suggest
{
    [UsedImplicitly]
    public class BingSuggest : ISuggestions
    {
        private readonly IRestClient _client;
        private readonly IMapper _mapper;
        public const int NumberOfEntries = 3;

        public BingSuggest(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        [Cache(TimeConst.Day, "autoSuggest", true)]
        public async Task<IEnumerable<string>> SuggestAsync(string query, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["mkt"] = "en-US",
                ["query"] = query,
            };
            var uri = new Uri("https://api.cognitive.microsoft.com/bing/v7.0/Suggestions");

            var result = await _client.GetAsync<SuggestionsObject>(uri, nvc, new[]
            {
                new KeyValuePair<string,string>("Ocp-Apim-Subscription-Key", "1ac3126fa3714e0089dc9132c0d1c14d")
            }, token).ConfigureAwait(false);
            return _mapper.Map<IEnumerable<string>>(result).Take(NumberOfEntries).ToArray();
        }


        public class SuggestionsObject
        {
            //public string _type { get; set; }
            //public Querycontext queryContext { get; set; }
            public Suggestiongroup[] SuggestionGroups { get; set; }
        }

        //public class Querycontext
        //{
        //    public string originalQuery { get; set; }
        //}

        public class Suggestiongroup
        {
            // public string name { get; set; }
            public Searchsuggestion[] SearchSuggestions { get; set; }
        }

        public class Searchsuggestion
        {
            // public string url { get; set; }
            public string DisplayText { get; set; }
            // public string query { get; set; }
            // public string searchKind { get; set; }
        }
    }
}
