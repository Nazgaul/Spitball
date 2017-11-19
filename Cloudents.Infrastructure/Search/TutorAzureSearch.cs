using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Entities;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search
{
    public class TutorAzureSearch : ITutorProvider
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public TutorAzureSearch(SearchServiceClient client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client.Indexes.GetClient("tutors");
        }

        public Task<IEnumerable<TutorDto>> SearchAsync(string term, SearchRequestFilter filter,
            SearchRequestSort sort, GeoPoint location, int page, CancellationToken token)
        {
            return SearchAzureAsync(term, filter, sort, location, page, token);
        }

        private async Task<IEnumerable<TutorDto>> SearchAzureAsync(string term,
            SearchRequestFilter filter, SearchRequestSort sort,
            GeoPoint location, int page, CancellationToken token)
        {
            if (sort == SearchRequestSort.Distance && location == null)
            {
                throw  new ArgumentException("Need to location");
            }
            if (filter == SearchRequestFilter.InPerson && location == null)
            {
                throw new ArgumentException("Need to location");
            }
            string filterQuery = null;
            var sortQuery = new List<string>();
            switch (filter)
            {
                case SearchRequestFilter.Online:
                    filterQuery = "online eq true";
                    break;
                case SearchRequestFilter.InPerson:
                    filterQuery = "inPerson eq true";
                    const double distance = 50 * 1.6;
                    sortQuery.Add($"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})') le {distance}");
                    break;
            }
            if (filter != SearchRequestFilter.InPerson)
            {
                switch (sort)
                {
                    case SearchRequestSort.Price:
                        sortQuery.Add("fee");
                        break;
                    case SearchRequestSort.Distance:
                        sortQuery.Add(
                            $"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})')");
                        break;
                    case SearchRequestSort.Rating:
                        sortQuery.Add("rank desc");
                        break;
                }
            }

            var searchParams = new SearchParameters
            {
                Top = 15,
                Skip = 15 * page,
                Select = new[]
                {
                    "name", "image", "url", "city", "state", "fee", "online", "location","subjects","extra"
                },
                Filter = filterQuery,
                OrderBy = sortQuery

            };
            var retVal = await
                _client.Documents.SearchAsync<Tutor>(term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return _mapper.Map<IEnumerable<Tutor>, IList<TutorDto>>(retVal.Results.Select(s => s.Document), opt => opt.Items["term"] = term);
        }
    }
}
