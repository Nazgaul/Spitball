﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Suggest
{
    [UsedImplicitly]
    public class TutorSuggest : ITutorSuggestion
    {
        private readonly ISearchIndexClient _client;
        private readonly IKeyGenerator _generator;

        public TutorSuggest(ISearchServiceClient client, IKeyGenerator generator)
        {
            _generator = generator;
            _client = client.Indexes.GetClient(AutoCompleteSearchWrite.IndexName);
        }

        public async Task<IEnumerable<string>> SuggestAsync(string query, CancellationToken token)
        {
            var searchParameter = new SearchParameters
            {
                Select = new List<string> { nameof(AutoComplete.Key) },
                Filter = $"{nameof(AutoComplete.Vertical)} eq {(int)Vertical.Tutor}",
                Top = BingSuggest.NumberOfEntries,
                ScoringProfile = AutoCompleteSearchWrite.ScoringProfile
            };
            var result = await _client.Documents.SearchAsync<AutoComplete>(query, searchParameter, cancellationToken: token).ConfigureAwait(false);

            return result.Results.Select(s => s.Document.Key);
        }

        public async Task<string> GetValueAsync(string query, CancellationToken token)
        {
            var key = _generator.GenerateKey(query.ToLowerInvariant());
            var result = await _client.Documents.GetAsync<AutoComplete>(key,new[] { nameof(AutoComplete.Value) },  cancellationToken: token).ConfigureAwait(false);
            return result?.Value;
        }
    }
}