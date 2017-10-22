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
        private readonly ISearchIndexClient m_Client;
        private readonly IMapper m_Mapper;
        //private readonly IRestClient m_RestClient;

        public TutorAzureSearch(SearchServiceClient client, IMapper mapper)
        {
            m_Mapper = mapper;
            m_Client = client.Indexes.GetClient("tutors");
        }

        public Task<IEnumerable<TutorDto>> SearchAsync(string term, SearchRequestFilter filter,
            SearchRequestSort sort, GeoPoint location, int page, CancellationToken token)
        {
            return SearchAzureAsync(term, filter, sort, location, page, token);
            
            //Task<IEnumerable<TutorDto>> taskTutorMe;
            //if (filter == SearchRequestFilter.InPerson)
            //{
            //    taskTutorMe = Task.FromResult<IEnumerable<TutorDto>>(new List<TutorDto>());
            //}
            //else
            //{
            //    taskTutorMe = TutorMeApiAsync(term, page, token);
            //}
            //await Task.WhenAll(taskAzure, taskTutorMe).ConfigureAwait(false);
            //return taskAzure.Result.Union(taskTutorMe.Result).OrderByDescending(o => o.TermFound);
        }

        //private async Task<IEnumerable<TutorDto>> TutorMeApiAsync(string term, int page, CancellationToken token)
        //{
        //    //https://gist.github.com/barbuza/4b3666fa88cd326f18f2c464c8e4487c
        //    // page is 12

        //    var nvc = new NameValueCollection
        //    {
        //        ["search"] = term,
        //        ["offset"] = (page * 12).ToString()
        //    };
        //    var result = await m_RestClient.GetAsync(new Uri("https://tutorme.com/api/v1/tutors/"), nvc, token).ConfigureAwait(false);
        //    return m_Mapper.Map<JObject, IEnumerable<TutorDto>>(result, opt => opt.Items["term"] = term);
        //}

        private async Task<IEnumerable<TutorDto>> SearchAzureAsync(string term,
            SearchRequestFilter filter, SearchRequestSort sort,
            GeoPoint location, int page, CancellationToken token)
        {
            string filterQuery = null;
            var sortQuery = new List<string>();
            switch (filter)
            {
                case SearchRequestFilter.Online:
                    filterQuery = "online eq true";
                    break;
                case SearchRequestFilter.InPerson:
                    filterQuery = "inPerson eq true";
                    break;
            }
            switch (sort)
            {
                case SearchRequestSort.Price:
                    sortQuery.Add("fee");
                    break;
                case SearchRequestSort.Distance when location != null:
                    sortQuery.Add($"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})')");
                    break;
                case SearchRequestSort.Rating:
                    sortQuery.Add("rank desc");
                    break;
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
                m_Client.Documents.SearchAsync<Tutor>(term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return m_Mapper.Map<IEnumerable<Tutor>, IList<TutorDto>>(retVal.Results.Select(s => s.Document), opt => opt.Items["term"] = term);
        }
    }
}
