using System.Collections.Generic;
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
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public UniversitySearch(ISearchServiceClient client, IMapper mapper)
        {
            _client = client.Indexes.GetClient("universities2");
            _mapper = mapper;
        }

        public async Task<IEnumerable<UniversityDto>> SearchAsync(string term, GeoPoint location,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 3)
            {
                term += "*";
            }
            var listOfSelectParams = new[] { "id", University.NameProperty, "imageField" };
            var searchParameter = new SearchParameters
            {
                Select = listOfSelectParams,
                Filter = "geographyPoint ne null",
                OrderBy = new List<string> { "search.score() desc", University.NameProperty }
            };
            if (location != null)
            {
                searchParameter.ScoringProfile = "university-score-location";
                searchParameter.ScoringParameters = new[] {
                    new ScoringParameter("currentLocation",GeographyPoint.Create(location.Latitude,location.Longitude))
                };
            }

            var tResult =
                _client.Documents.SearchAsync<University>(term, searchParameter,
                    cancellationToken: token);

            var tSuggest = CompletedTask;
            if (term.Length >= 3)
            {
                tSuggest = _client.Documents.SuggestAsync<University>(term, "sg",
                    new SuggestParameters
                    {
                        UseFuzzyMatching = true,
                        Select = listOfSelectParams,
                        Filter = "geographyPoint ne null"
                    }, cancellationToken: token);
            }
            await Task.WhenAll(tResult, tSuggest).ConfigureAwait(false);

            var result = _mapper.Map<IEnumerable<University>, IList<UniversityDto>>(tResult.Result.Results.Select(s => s.Document));
            if (tSuggest.Result != null)
            {
                var result2 =
                    _mapper.Map<IEnumerable<University>, IList<UniversityDto>>(
                        tSuggest?.Result?.Results?.Select(s => s.Document));
                return result.Union(result2, new UniversityDtoEquality());
            }
            return result;
        }

        private static readonly Task<DocumentSuggestResult<University>> CompletedTask = Task.FromResult<DocumentSuggestResult<University>>(null);
    }
}

