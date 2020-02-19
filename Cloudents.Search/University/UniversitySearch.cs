using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Search.University
{
    [UsedImplicitly]
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ISearchIndexClient _client;

        private const int PageSize = 30;

        private readonly string[] _listOfSelectParams = {
            nameof(Entities.University.Id),
            nameof(Entities.University.DisplayName),
            nameof(Entities.University.Country),
            nameof(Entities.University.Image),
            Entities.University.UserCountFieldName
        };

        public UniversitySearch(ISearchService client)
        {
            _client = client.GetClient(UniversitySearchWrite.IndexName);
        }

        public async Task<UniversitySearchDto> SearchAsync(string term, int page, [CanBeNull] string country,
            CancellationToken token)
        {
            string CountryFilter(string country1)
            {
                return $"{nameof(Entities.University.Country)} eq '{country1.ToUpperInvariant()}'";
            }

            var searchParameter = new SearchParameters
            {
                Select = _listOfSelectParams,
                Top = PageSize,
                Skip = PageSize * page,
                OrderBy = new List<string> { "search.score() desc",
                    $"{Entities.University.UserCountFieldName} desc" }

            };
            if (country != null)
            {
                searchParameter.Filter = CountryFilter(country);
            }

            term = term?.Replace("\"", "\\");
            var searchDocumentResult = await
                _client.Documents.SearchAsync<Entities.University>(term, searchParameter,
                    cancellationToken: token);

            if (searchDocumentResult.Results.Count != 0)
            {
                return new UniversitySearchDto(searchDocumentResult.Results.Select(s =>
                    ToDto(s.Document)));
            }

            if (page == 0 && !string.IsNullOrEmpty(term))
            {
                var suggestParams = new SuggestParameters()
                {
                    Select = _listOfSelectParams,
                    UseFuzzyMatching = true,
                    Top = PageSize,
                };
                if (country != null)
                {
                    suggestParams.Filter = CountryFilter(country);
                }
                var suggesterResult = await _client.Documents.SuggestAsync<Entities.University>(term,
                    UniversitySearchWrite.SuggesterName, suggestParams
                    , cancellationToken: token);


                return new UniversitySearchDto(suggesterResult.Results.Select(s =>
                    ToDto(s.Document)));
            }

            return new UniversitySearchDto(Enumerable.Empty<UniversityDto>());
        }

        private static UniversityDto ToDto(Entities.University university)
        {
            return new UniversityDto(Guid.Parse(university.Id), university.DisplayName, university.Country,
                university.Image, university.UsersCount.GetValueOrDefault());
        }
    }
}

