using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Entities;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search
{
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ISearchIndexClient m_Client;
        private readonly IMapper m_Mapper;

        public UniversitySearch(ISearchServiceClient client, IMapper mapper)
        {
            m_Client = client.Indexes.GetClient("universities2"); ;
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
            var sortQuery = new List<string>();

            if (location != null)
            {
                sortQuery.Add($"geo.distance(geographyPoint, geography'POINT({location.Longitude} {location.Latitude})')");
            }
            var searchParameter = new SearchParameters
            {
                Select = listOfSelectParams,
                Filter = "geographyPoint ne null",
                OrderBy = sortQuery
            };

            if (!term.Contains(" "))
            {
                term = "*";
            }
            //if (string.IsNullOrEmpty(term)) //obsolete
            //{
            //    searchParameter.ScoringProfile = CountryScoringProfile;
            //    searchParameter.ScoringParameters = new[] { new ScoringParameter("country", new[] { query.Country }) };
            //}
            //else
            //{
            //    term = query.Term.Replace("\"", string.Empty);
            //}

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

