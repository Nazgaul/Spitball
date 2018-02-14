using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Write.Tutor;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using TutorObj = Cloudents.Core.Entities.Search.Tutor;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public class TutorAzureSearch : ITutorProvider
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public TutorAzureSearch(ISearchServiceClient client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client.Indexes.GetClient(TutorSearchWrite.IndexName);
        }

        public async Task<IEnumerable<TutorDto>> SearchAsync(string term, TutorRequestFilter[] filters,
            TutorRequestSort sort, GeoPoint location, int page, bool isMobile, CancellationToken token)
        {
            var sortQuery = new List<string>();
            var filterQuery = ApplyFilter(filters, location);
            switch (sort)
            {
                case TutorRequestSort.Price:
                    sortQuery.Add(nameof(TutorObj.Fee));
                    break;
                case TutorRequestSort.Distance:
                    sortQuery.Add(
                        $"geo.distance({nameof(TutorObj.Location)}, geography'POINT({location.Longitude} {location.Latitude})')");
                    break;
            }

            var searchParams = new SearchParameters
            {
                Top = TutorSearch.PageSize,
                Skip = TutorSearch.PageSize * page,
                Select = new[]
                {
                    nameof(TutorObj.Name),
                    nameof(TutorObj.Image),
                    nameof(TutorObj.Url),
                    nameof(TutorObj.City),
                    nameof(TutorObj.State),
                    nameof(TutorObj.Fee),
                    nameof(TutorObj.TutorFilter),
                    nameof(TutorObj.Location),
                    nameof(TutorObj.Description),
                    nameof(TutorObj.Source)
                },
                Filter = string.Join(" and ", filterQuery),
                OrderBy = sortQuery

            };
            var retVal = await
                _client.Documents.SearchAsync<TutorObj>(term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return _mapper.Map<IEnumerable<TutorObj>, IList<TutorDto>>(retVal.Results.Select(s => s.Document), opt => opt.Items["term"] = term);
        }

        private static IEnumerable<string> ApplyFilter(IEnumerable<TutorRequestFilter> filters, GeoPoint location)
        {
            var filterQuery = new List<string>();
            var filterResult = TutorFilter.None;
            foreach (var filter in filters ?? Enumerable.Empty<TutorRequestFilter>())
            {
                switch (filter)
                {
                    case TutorRequestFilter.Online:
                        filterResult |= TutorFilter.Online;
                        break;
                    case TutorRequestFilter.InPerson:
                        filterResult |= TutorFilter.InPerson;
                        const double distance = 50 * 1.6;
                        filterQuery.Add(
                            $"geo.distance({nameof(TutorObj.Location)}, geography'POINT({location.Longitude} {location.Latitude})') le {distance}");
                        break;
                }
            }

            if (filterResult != TutorFilter.None)
            {
                filterQuery.Add($"{nameof(TutorObj.TutorFilter)} eq {(int)filterResult}");
            }

            return filterQuery;
        }
    }
}
