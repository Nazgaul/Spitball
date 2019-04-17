//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core;
//using Cloudents.Core.Attributes;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Models;
//using Cloudents.Infrastructure.Search.Tutor;
//using Microsoft.Azure.Search;
//using Microsoft.Azure.Search.Models;

//namespace Cloudents.Search.Tutor
//{
//    public class TutorAzureSearch : ITutorProvider
//    {
//        private readonly ISearchIndexClient _client;
//        private static readonly int PageSize = (int)PrioritySource.TutorMe.Priority * TutorMeSearch.TutorMePage;

//        public TutorAzureSearch(ISearchService client)
//        {
//            _client = client.GetOldClient(TutorSearchWrite.IndexName);
//        }

//        [Log]
//        public async Task<IEnumerable<TutorDto>> SearchAsync(string term, TutorRequestFilter[] filters,
//            TutorRequestSort sort, GeoPoint location, int page, bool isMobile, CancellationToken token)
//        {
//            var sortQuery = new List<string>();
//            var filterQuery = ApplyFilter(filters, location);
//            switch (sort)
//            {
//                case TutorRequestSort.Price:
//                    sortQuery.Add(nameof(Entities.Tutor.Fee));
//                    break;
                  
//            }


//            var searchParams = new SearchParameters
//            {
//                Top = PageSize,
//                Skip = PageSize * page,
//                Select = new[]
//                {
//                    nameof(Entities.Tutor.Name),
//                    nameof(Entities.Tutor.Image),
//                    nameof(Entities.Tutor.Url),
//                    nameof(Entities.Tutor.City),
//                    nameof(Entities.Tutor.State),
//                    nameof(Entities.Tutor.Fee),
//                    nameof(Entities.Tutor.TutorFilter),
//                    nameof(Entities.Tutor.Location),
//                    nameof(Entities.Tutor.Description),
//                    nameof(Entities.Tutor.Source)
//                },
//                Filter = string.Join(" or ", filterQuery),
//                OrderBy = sortQuery

//            };
//            var retVal = await
//                _client.Documents.SearchAsync<Entities.Tutor>(term, searchParams, cancellationToken: token);

//            return retVal.Results.Select((s, i) => new TutorDto
//            {
//                State = s.Document.State,
//                Name = s.Document.Name,
//                Url = s.Document.Url,
//                Image = s.Document.Image,
//                Order = i + 1,
//                City = s.Document.City,
//                Description = s.Document.Description,
//                Fee = s.Document.Fee,
//                Online = s.Document.TutorFilter == TutorFilter.Online,
//                PrioritySource = PrioritySource.TutorWyzant
//            });
//        }

//        private static IEnumerable<string> ApplyFilter(IEnumerable<TutorRequestFilter> filters, GeoPoint location)
//        {
//            var filterQuery = new List<string>();
//            var filterResult = TutorFilter.None;
//            foreach (var filter in filters ?? Enumerable.Empty<TutorRequestFilter>())
//            {
//                switch (filter)
//                {
//                    case TutorRequestFilter.Online:
//                        filterResult |= TutorFilter.Online;
//                        break;
//                    case TutorRequestFilter.InPerson:
//                        filterResult |= TutorFilter.InPerson;
//                        const double distance = 50 * 1.6;
//                        filterQuery.Add(
//                            $"geo.distance({nameof(Entities.Tutor.Location)}, geography'POINT({location.Longitude} {location.Latitude})') le {distance}");
//                        break;
//                }
//            }

//            if (filterResult != TutorFilter.None)
//            {
//                filterQuery.Add($"{nameof(Entities.Tutor.TutorFilter)} eq {(int)filterResult}");
//            }

//            return filterQuery;
//        }
//    }
//}
