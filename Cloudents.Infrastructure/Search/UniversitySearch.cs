using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Write;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;

namespace Cloudents.Infrastructure.Search
{
    [UsedImplicitly]
    public class UniversitySearch : IUniversitySearch
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        private readonly string[] _listOfSelectParams = {
            nameof(University.Id),
            nameof(University.Name),
            nameof(University.Image)
        };

        public UniversitySearch(ISearchServiceClient client, IMapper mapper)
        {
            _client = client.Indexes.GetClient(UniversitySearchWrite.IndexName);
            _mapper = mapper;
        }

        public async Task<UniversityDto> GetApproximateUniversitiesAsync(GeoPoint location,
            CancellationToken token)
        {
            if (location == null)
            {
                throw new System.ArgumentNullException(nameof(location));
            }

            var searchParameter = new SearchParameters
            {
                Select = _listOfSelectParams,
                Top = 1,
                Filter =
                    $"geo.distance({nameof(University.GeographyPoint)}, geography'POINT({location.Longitude} {location.Latitude})') le 5"
            };
            var result = await
                _client.Documents.SearchAsync<University>(null, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

            var university = result.Results.FirstOrDefault()?.Document;

            return _mapper.Map<UniversityDto>(university);
        }

        public async Task<IEnumerable<UniversityDto>> SearchAsync(string term, GeoPoint location,
            CancellationToken token)
        {
            //geo.distance({nameof(University.GeographyPoint)}, geography'POINT({location.Longitude} {location.Latitude})')
            var sort = new List<string> {"search.score() desc"};
            var searchParameter = new SearchParameters
            {
                Select = _listOfSelectParams,
                Top = 15,
                //OrderBy = new List<string> { "search.score() desc", nameof(University.Name) }
            };
            if (location != null)
            {
                sort.Add($"geo.distance({nameof(University.GeographyPoint)}, geography'POINT({location.Longitude} {location.Latitude})')");
                searchParameter.ScoringProfile = UniversitySearchWrite.ScoringProfile;
                searchParameter.ScoringParameters = new[] {
                    new ScoringParameter
                        (UniversitySearchWrite.DistanceScoringParameter
                        ,GeographyPoint.Create(location.Latitude,location.Longitude))
                };
            }
            sort.Add(nameof(University.Name));
            searchParameter.OrderBy = sort;

            var result = await
                _client.Documents.SearchAsync<University>(term, searchParameter,
                    cancellationToken: token).ConfigureAwait(false);

            return _mapper.Map<IEnumerable<UniversityDto>>(result.Results.Select(s => s.Document));
        }
    }
}

