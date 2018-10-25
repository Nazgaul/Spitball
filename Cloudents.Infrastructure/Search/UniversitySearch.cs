using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Write;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Search
{
    [UsedImplicitly]
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ISearchIndexClient _client;

        private readonly string[] _listOfSelectParams = {
            nameof(University.Id),
            nameof(University.DisplayName),
            nameof(University.Country)
        };

        public UniversitySearch(ISearchService client)
        {
            _client = client.GetClient(UniversitySearchWrite.IndexName);
        }


        private static readonly string[] StopWordsList = { "university","university of",
            "college",
            "school",
            "Community",
            "High",
            "Uni",
            "State",
            "המכללה","אוניברסיטת","מכללת","האוניברסיטה"
        };


        public async Task<UniversitySearchDto> SearchAsync(string term, string country,
            CancellationToken token)
        {
            if (term.Contains(StopWordsList, StringComparison.InvariantCultureIgnoreCase))
            {
                return UniversitySearchDto.StopWordResponse();
            }
            var searchParameter = new SearchParameters
            {
                Select = _listOfSelectParams,
                Top = 15,
                //QueryType = QueryType.Full,
                OrderBy = new List<string> { "search.score() desc", nameof(University.DisplayName) },
                ScoringProfile = UniversitySearchWrite.ScoringProfile,
                ScoringParameters = new[]
                {
                    new ScoringParameter
                    (UniversitySearchWrite.CountryTagScoringParameters
                        , new[] {country})
                }
            };

            term = term.Replace("\"", "\\");
            var result = await
                _client.Documents.SearchAsync<University>(term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);
            return new UniversitySearchDto(result.Results.Select(s =>
                new UniversityDto(Guid.Parse(s.Document.Id), s.Document.DisplayName, s.Document.Country)));
        }
    }
}

