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
    public class JobSearch : IJobSearch
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        public const int PageSize = 30;

        public JobSearch(ISearchServiceClient client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client.Indexes.GetClient(JobSearchWrite.IndexName);
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(
            IEnumerable<string> term,
            JobRequestFilter filter,
            JobRequestSort sort,
            IEnumerable<string> jobType,
            Location location,
            int page,
            CancellationToken token)
        {
            var filterQuery = new List<string>();
            var sortQuery = new List<string>();
            if (filter == JobRequestFilter.Paid)
            {
                filterQuery.Add($"{nameof(Entity.Job.Compensation)} eq 'paid'");
            }

            if (jobType != null)
            {
                filterQuery.AddRange(jobType.Select(s => $"{nameof(Entity.Job.JobType)} eq '{s}'"));
            }

            switch (sort)
            {
                case JobRequestSort.Distance when location != null:
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
                Top = PageSize,
                Skip = PageSize * page,
                Filter = string.Join(" or ", filterQuery),
                OrderBy = sortQuery

            };
            var str = string.Join(" ", term ?? Enumerable.Empty<string>());
            if (string.IsNullOrWhiteSpace(str))
            {
                str = "*";
            }

            var retVal = await
                _client.Documents.SearchAsync<Entity.Job>(str, searchParams, cancellationToken: token).ConfigureAwait(false);
            return _mapper.Map<ResultWithFacetDto<JobDto>>(retVal,
                opt => opt.Items[JobResultConverter.FacetType] = nameof(Entity.Job.JobType));
        }
    }
}
