﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Entities;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;

namespace Cloudents.Infrastructure.Search
{
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ISearchIndexClient m_Client;
        private readonly IMapper m_Mapper;

        public UniversitySearch(ISearchServiceClient client, IMapper mapper)
        {
            m_Client = client.Indexes.GetClient("universities2");
            m_Mapper = mapper;
        }

        public async Task<IEnumerable<UniversityDto>> SearchAsync(string term, GeoPoint location,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(term))
            {
                term = "*";
            }
            var listOfSelectParams = new[] { "id", "name3", "imageField" };
            var searchParameter = new SearchParameters
            {
                Select = listOfSelectParams,
                Filter = "geographyPoint ne null"
            };
            if (location != null)
            {
                searchParameter.ScoringProfile = "university-score-location";
                searchParameter.ScoringParameters = new[] {
                    new ScoringParameter("currentLocation",GeographyPoint.Create(location.Latitude,location.Longitude))
                };
            }

            var tResult =
                m_Client.Documents.SearchAsync<University>(term, searchParameter,
                    cancellationToken: token);

            var tSuggest = CompletedTask;
            if (!string.IsNullOrEmpty(term) && term.Length >= 3)
            {
                tSuggest = m_Client.Documents.SuggestAsync<University>(term, "sg",
                    new SuggestParameters
                    {
                        UseFuzzyMatching = true,
                        Select = listOfSelectParams
                    }, cancellationToken: token);
            }
            await Task.WhenAll(tResult, tSuggest).ConfigureAwait(false);

            var result = m_Mapper.Map<IEnumerable<University>, IList<UniversityDto>>(tResult.Result.Results.Select(s => s.Document));
            var result2 = m_Mapper.Map<IEnumerable<University>, IList<UniversityDto>>(tSuggest?.Result?.Results.Select(s => s.Document));
            return result.Union(result2);
        }

        private static readonly Task<DocumentSuggestResult<University>> CompletedTask = Task.FromResult<DocumentSuggestResult<University>>(null);
    }
}

