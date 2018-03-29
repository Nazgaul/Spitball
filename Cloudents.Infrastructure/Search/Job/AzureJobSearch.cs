﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Entity = Cloudents.Core.Entities.Search;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Write.Job;
using JetBrains.Annotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    [UsedImplicitly]
    public class AzureJobSearch : IJobProvider
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public AzureJobSearch(ISearchServiceClient client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client.Indexes.GetClient(JobSearchWrite.IndexName);
        }

        [Cache(TimeConst.Hour, "job-azure", false)]
        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(JobProviderRequest jobProviderRequest, CancellationToken token)
        {
            var filterQuery = new List<string>();
            var sortQuery = new List<string>();

            if (jobProviderRequest.JobType != null)
            {
                var filterStr = string.Join(" or ", jobProviderRequest.JobType.Select(s =>
                    $"{nameof(Entity.Job.JobType)} eq '{s.GetDescription()}'"));
                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                    filterStr = $"({filterStr})";
                }
                filterQuery.Add(filterStr);
            }

            switch (jobProviderRequest.Sort)
            {
                case JobRequestSort.Relevance when jobProviderRequest.Location?.Point != null:
                    filterQuery.Add($"geo.distance({ nameof(Entity.Job.Location)}, geography'POINT({jobProviderRequest.Location.Point.Longitude} {jobProviderRequest.Location.Point.Latitude})') le {JobSearch.RadiusOfFindingJobKm}");
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
                Skip = JobSearch.PageSize * jobProviderRequest.Page,
                Filter = string.Join(" and ", filterQuery),
                OrderBy = sortQuery
            };

            var retVal = await
                _client.Documents.SearchAsync<Entity.Job>(jobProviderRequest.Term, searchParams, cancellationToken: token).ConfigureAwait(false);
            return _mapper.Map<ResultWithFacetDto<JobDto>>(retVal);
        }
    }
}
