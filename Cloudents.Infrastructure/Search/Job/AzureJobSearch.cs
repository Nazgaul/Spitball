﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entity = Cloudents.Core.Entities.Search;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Write.Job;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    public class AzureJobSearch : IJobProvider
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public AzureJobSearch(ISearchServiceClient client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client.Indexes.GetClient(JobSearchWrite.IndexName);
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(
            string term,
            JobRequestSort sort,
            IEnumerable<string> jobType,
            Location location,
            int page,
            bool highlight,
            CancellationToken token)
        {
            var filterQuery = new List<string>();
            var sortQuery = new List<string>();

            if (jobType != null)
            {
                filterQuery.AddRange(jobType.Select(s => $"{nameof(Entity.Job.JobType)} eq '{s}'"));
            }

            switch (sort)
            {
                case JobRequestSort.Distance when location?.Point != null:
                    sortQuery.Add($"geo.distance({ nameof(Entity.Job.Location)}, geography'POINT({location.Point.Longitude} {location.Point.Latitude})')");
                    break;
                case JobRequestSort.Date:
                    sortQuery.Add($"{nameof(Entity.Job.DateTime)} desc");
                    break;
            }
            var searchParams = new SearchParameters
            {
                Select = new[]
                {
                    nameof(Entity.Job.Title),
                    nameof(Entity.Job.Description),
                    nameof(Entity.Job.DateTime),
                    nameof(Entity.Job.City),
                    nameof(Entity.Job.State),
                    nameof(Entity.Job.JobType),
                    nameof(Entity.Job.Compensation),
                    nameof(Entity.Job.Url),
                    nameof(Entity.Job.Company),
                    nameof(Entity.Job.Source)
                },
                Facets = filterQuery.Count == 0 ? new[]
                {
                    nameof(Entity.Job.JobType)
                } : null,
                Top = JobSearch.PageSize,
                Skip = JobSearch.PageSize * page,
                Filter = string.Join(" or ", filterQuery),
                OrderBy = sortQuery

            };
            if (string.IsNullOrWhiteSpace(term))
            {
                term = "*";
            }

            var retVal = await
                _client.Documents.SearchAsync<Entity.Job>(term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return _mapper.Map<ResultWithFacetDto<JobDto>>(retVal);
        }
    }
}
