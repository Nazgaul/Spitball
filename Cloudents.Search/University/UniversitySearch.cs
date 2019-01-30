﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.University
{
    [UsedImplicitly]
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ISearchIndexClient _client;

        private readonly string[] _listOfSelectParams = {
            nameof(Entities.University.Id),
            nameof(Entities.University.DisplayName),
            nameof(Entities.University.Country)
        };

        public UniversitySearch(ISearchService client)
        {
            _client = client.GetClient(UniversitySearchWrite.IndexName);
        }

        public async Task<UniversitySearchDto> SearchAsync(string term, string country,
            CancellationToken token)
        {
            var searchParameter = new SearchParameters
            {
                Select = _listOfSelectParams,
                Top = 15,
                OrderBy = new List<string> { "search.score() desc", nameof(Entities.University.DisplayName) },
                ScoringProfile = UniversitySearchWrite.ScoringProfile,
                ScoringParameters = new[]
                {
                    new ScoringParameter
                    (UniversitySearchWrite.CountryTagScoringParameters
                        , new[] {country})
                }
            };

            term = term?.Replace("\"", "\\");
            var result = await
                _client.Documents.SearchAsync<Entities.University>(term, searchParameter,
                    cancellationToken: token);
            return new UniversitySearchDto(result.Results.Select(s =>
                new UniversityDto(Guid.Parse(s.Document.Id), s.Document.DisplayName, s.Document.Country)));
        }
    }
}

