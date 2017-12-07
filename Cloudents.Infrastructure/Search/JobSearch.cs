﻿using System.Collections.Generic;
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
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        private const int PageSize = 30;

        public JobSearch(SearchServiceClient client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client.Indexes.GetClient("jobs");
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(
            string term,
            JobRequestFilter filter,
            JobRequestSort sort,
            IEnumerable<string> jobType,
            GeoPoint location,
            int page,
            CancellationToken token)
        {
            string filterQuery = null;
            var sortQuery = new List<string>();
            if (filter == JobRequestFilter.Paid)
            {
                filterQuery = "compensationType eq 'paid'";
            }

            if (jobType != null)
            {
                filterQuery = string.Join(" or ", jobType.Select(s => $"jobType eq '{s}'"));
            }

            switch (sort)
            {
                case JobRequestSort.Distance when location != null:
                    sortQuery.Add($"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})')");
                    break;
                case JobRequestSort.Date:
                    sortQuery.Add("dateTime");
                    break;
            }
            var searchParams = new SearchParameters
            {
                Select = new[]
                {
                    "title","responsibilities","dateTime","city","state","jobType","compensationType","url","company"
                },
                Facets = string.IsNullOrEmpty(filterQuery) ? new[]
                {
                    "jobType"
                } : null,
                Top = PageSize,
                Skip = PageSize * page,
                Filter = filterQuery,
                OrderBy = sortQuery

            };
            if (string.IsNullOrEmpty(term))
            {
                term = "*";
            }

            var retVal = await
                _client.Documents.SearchAsync<Job>(term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return _mapper.Map<ResultWithFacetDto<JobDto>>(retVal,
                opt => opt.Items[JobResultConverter.FacetType] = "jobType");
        }
    }
}
