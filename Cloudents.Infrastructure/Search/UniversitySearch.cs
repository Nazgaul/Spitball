using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Entities;
using Cloudents.Infrastructure.Write;
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
            _client = client.Indexes.GetClient(UniversitySearchWrite.IndexName);
            _mapper = mapper;
        }

        public async Task<IEnumerable<UniversityDto>> SearchAsync(string term, GeoPoint location,
            CancellationToken token)
        {
            //if (string.IsNullOrEmpty(term) || term.Length < 3)
            //{
            //    term += "*";
            //}
            var listOfSelectParams = new[]
            {
                nameof(University.Id),
                nameof(University.Name),
                nameof(University.Image)
            };
            var searchParameter = new SearchParameters
            {
                Select = listOfSelectParams,
                OrderBy = new List<string> { "search.score() desc", nameof(University.Name) }
            };
            if (location != null)
            {
                searchParameter.ScoringProfile = UniversitySearchWrite.ScoringProfile;
                searchParameter.ScoringParameters = new[] {
                    new ScoringParameter
                        (UniversitySearchWrite.DistanceScoringParameter
                        ,GeographyPoint.Create(location.Latitude,location.Longitude))
                };
            }

            var result = await
                _client.Documents.SearchAsync<University>(term, searchParameter,
                    cancellationToken: token);

            return _mapper.Map<IEnumerable<University>, IList<UniversityDto>>(result.Results.Select(s => s.Document));
            //if (tSuggest.Result != null)
            //{
            //    var result2 =
            //        _mapper.Map<IEnumerable<University>, IList<UniversityDto>>(
            //            tSuggest?.Result?.Results?.Select(s => s.Document));
            //    return result.Union(result2, new UniversityDtoEquality());
            //}
            //return result;
        }

        private static readonly Task<DocumentSuggestResult<University>> CompletedTask = Task.FromResult<DocumentSuggestResult<University>>(null);
    }
}

