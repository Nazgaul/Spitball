using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Search.Entities;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Search
{
    public class TutorSearch : ITutorSearch
    {
        private readonly ISearchIndexClient m_Client;
        private readonly IMapper m_Mapper;
        private readonly IRestClient m_RestClient;

        public TutorSearch(SearchServiceClient client, IMapper mapper, IRestClient restClient)
        {
            m_Mapper = mapper;
            m_RestClient = restClient;
            m_Client = client.Indexes.GetClient("tutors");
        }

        public async Task<IEnumerable<TutorDto>> SearchAsync(string term, SearchRequestFilter filter, SearchRequestSort sort, GeoPoint location, CancellationToken token)
        {
            var taskAzure = SearchAzureAsync(term, filter, sort, location, token);
            Task<IEnumerable<TutorDto>> taskTutorMe;
            if (filter == SearchRequestFilter.InPerson)
            {
                taskTutorMe = Task.FromResult<IEnumerable<TutorDto>>(new List<TutorDto>());
            }
            else
            {
                taskTutorMe = TutorMeApiAsync(term, token);
            }
            await Task.WhenAll(taskAzure, taskTutorMe).ConfigureAwait(false);
            return taskAzure.Result.Union(taskTutorMe.Result).OrderByDescending(o => o.TermFound);
        }

        private async Task<IEnumerable<TutorDto>> TutorMeApiAsync(string term, CancellationToken token)
        {

            var nvc = new NameValueCollection
            {
                ["search"] = term
            };
            var result = await m_RestClient.GetAsync(new Uri("https://tutorme.com/api/v1/tutors/"), nvc, token).ConfigureAwait(false);
            return m_Mapper.Map<JObject, IEnumerable<TutorDto>>(result, opt => opt.Items["term"] = term);
           
            //using (var client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    var uri = new UriBuilder("https://tutorme.com/api/v1/tutors/");
            //    var nvc = new NameValueCollection
            //    {
            //        ["search"] = term
            //    };
            //    uri.AddQuery(nvc);

            //    var response = await client.GetAsync(uri.Uri, token).ConfigureAwait(false);
            //    if (!response.IsSuccessStatusCode) return retVal;
            //    var str = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            //    var o = JObject.Parse(str);
            //    retVal.AddRange(o["results"].Children()
            //        .Select(result => new TutorDto
            //        {
            //            Url = $"https://tutorme.com/tutors/{result["id"].Value<string>()}",
            //            Image = result["avatar"]["x300"].Value<string>(),
            //            Name = result["shortName"].Value<string>(),
            //            Online = result["isOnline"].Value<bool>(),
            //            TermFound = result.ToString().Split(new[] {term},StringSplitOptions.RemoveEmptyEntries).Length
            //        }));
            //}
            //return retVal;
        }

        private async Task<IList<TutorDto>> SearchAzureAsync(string term,
            SearchRequestFilter filter, SearchRequestSort sort,
            GeoPoint location, CancellationToken token)
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
