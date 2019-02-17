using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Suggest;
using Cloudents.Search.AutoComplete;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Tutor
{
    [UsedImplicitly]
    public class TutorSuggest : ITutorSuggestion
    {
        private readonly ISearchIndexClient _client;
        private readonly IKeyGenerator _generator;
        private readonly Vertical _vertical;

        public const string VerticalParameter = "vertical";

        public TutorSuggest(ISearchService client, IKeyGenerator generator, Vertical vertical)
        {
            _generator = generator;
            _vertical = vertical;
            _client = client.GetOldClient(AutoCompleteSearchWrite.IndexName);
        }

        public async Task<IEnumerable<string>> SuggestAsync(string query, CancellationToken token)
        {
            var searchParameter = new SearchParameters
            {
                Select = new List<string> { nameof(Entities.AutoComplete.Key) },
                Filter = $"{nameof(Entities.AutoComplete.Vertical)} eq {(int)_vertical}",
                Top = BingSuggest.NumberOfEntries,
                ScoringProfile = AutoCompleteSearchWrite.ScoringProfile
            };
            var result = await _client.Documents.SearchAsync<Entities.AutoComplete>(query, searchParameter, cancellationToken: token).ConfigureAwait(false);

            return result.Results.Select(s => s.Document.Key);
        }

        public async Task<string> GetValueAsync(string query, CancellationToken token)
        {
            var key = _generator.GenerateKey(query.ToLowerInvariant());
            try
            {
                var result = await _client.Documents
                    .GetAsync<Entities.AutoComplete>(key, new[] { nameof(Entities.AutoComplete.Value) }, cancellationToken: token);
                return result?.Value;
            }
            catch (Microsoft.Rest.Azure.CloudException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}