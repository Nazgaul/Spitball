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
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Converters;

namespace Cloudents.Infrastructure.Search
{
    public class JobSearch : IJobSearch
    {
        private readonly ISearchIndexClient m_Client;
        private readonly IMapper m_Mapper;

        public JobSearch(SearchServiceClient client, IMapper mapper)
        {
            m_Mapper = mapper;
            m_Client = client.Indexes.GetClient("jobs");
        }

        public async Task<JobFacetDto> SearchAsync(
            string term,
            SearchRequestFilter filter,
            SearchRequestSort sort,
            string jobType,
            GeoPoint location,
            CancellationToken token)
        {
            string filterQuery = null;
            var sortQuery = new List<string>();
            if (filter == SearchRequestFilter.Paid)
            {
                filterQuery = "compensationType eq 'paid'";
            }
            if (!string.IsNullOrEmpty(jobType))
            {
                filterQuery = $"jobType eq {jobType}";
            }

            switch (sort)
            {
                case SearchRequestSort.Distance when location != null:
                    sortQuery.Add($"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})')");
                    break;
                case SearchRequestSort.Date:
                    sortQuery.Add("dateTime");
                    break;
            }
            var searchParams = new SearchParameters
            {
                Select = new[]
                {
                    "title","responsibilities","dateTime","city","state","jobType","compensationType","url","company"
                },
                Facets = filterQuery != null || sortQuery.Count > 0 ? null : new[]
                {
                    "jobType"
                },
                Filter = filterQuery,
                OrderBy = sortQuery

            };

            var retVal = await
                m_Client.Documents.SearchAsync<Job>(term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return m_Mapper.Map<JobFacetDto>(retVal, opt => opt.Items[JobResultConverter.FacetType] = "jobType");
        }
    }
}
