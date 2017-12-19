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
            _client = client.Indexes.GetClient("tutors2");
        }

        public async Task<IEnumerable<TutorDto>> SearchAsync(string term, TutorRequestFilter[] filters,
            TutorRequestSort sort, GeoPoint location, int page, CancellationToken token)
        {
            var filterQuery = new List<string>();
            var sortQuery = new List<string>();

            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case TutorRequestFilter.Online:
                        filterQuery.Add("online eq true");
                        break;
                    case TutorRequestFilter.InPerson:
                        filterQuery.Add("inPerson eq true");
                        const double distance = 50 * 1.6;
                        filterQuery.Add(
                            $"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})') le {distance}");
                        break;
                }
            }
            switch (sort)
            {
                case TutorRequestSort.Price:
                    sortQuery.Add("fee");
                    break;
                case TutorRequestSort.Distance:
                    sortQuery.Add(
                        $"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})')");
                    break;
            }

            var searchParams = new SearchParameters
            {
                Top = 15,
                Skip = 15 * page,
                Select = new[]
                {
                    "name", "image", "url", "city", "state", "fee", "online", "location","subjects","extra","description"
                },
                Filter = string.Join(" and ", filterQuery),
                OrderBy = sortQuery

            };
            var retVal = await
                _client.Documents.SearchAsync<Tutor>(term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return _mapper.Map<IEnumerable<Tutor>, IList<TutorDto>>(retVal.Results.Select(s => s.Document), opt => opt.Items["term"] = term);
        }
    }
}
