using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Application;
using Cloudents.Application.Attributes;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Application.Extension;
using Cloudents.Application.Models;
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

        public AzureJobSearch(ISearchService client, IMapper mapper)
        {
            _mapper = mapper;
            _client = client.GetOldClient(JobSearchWrite.IndexName);
        }

        [Cache(TimeConst.Hour, "job-azure", false)]
        [Log]
        public async Task<ResultWithFacetDto<JobProviderDto>> SearchAsync(JobProviderRequest jobProviderRequest, CancellationToken token)
        {
            var sortQuery = new List<string>();

            var filterQuery = BuildFilter(jobProviderRequest.JobType, jobProviderRequest.Location);

            if (jobProviderRequest.Sort == JobRequestSort.Date)
            {
                sortQuery.Add($"{nameof(Application.Entities.Search.Job.DateTime)} desc");
            }
            var searchParams = new SearchParameters
            {
                Select = new[]
                {
                    nameof(Application.Entities.Search.Job.Title),
                    nameof(Application.Entities.Search.Job.Description),
                    nameof(Application.Entities.Search.Job.DateTime),
                    nameof(Application.Entities.Search.Job.City),
                    nameof(Application.Entities.Search.Job.State),
                    nameof(Application.Entities.Search.Job.JobType),
                    nameof(Application.Entities.Search.Job.Url),
                    nameof(Application.Entities.Search.Job.Company),
                    nameof(Application.Entities.Search.Job.Source)
                },
                Facets = filterQuery.Count == 0 ? new[]
                {
                    nameof(Application.Entities.Search.Job.JobType)
                } : null,
                Top = JobSearch.PageSize,
                Skip = JobSearch.PageSize * jobProviderRequest.Page,
                Filter = string.Join(" and ", filterQuery),
                OrderBy = sortQuery
            };

            var retVal = await
                _client.Documents.SearchAsync<Application.Entities.Search.Job>(jobProviderRequest.Term, searchParams, cancellationToken: token).ConfigureAwait(false);
            if (retVal.Results.Count == 0)
            {
                return null;
            }
            return _mapper.Map<ResultWithFacetDto<JobProviderDto>>(retVal);
        }

        private static IList<string> BuildFilter(IEnumerable<JobFilter> filters, Location location)
        {
            var filterQuery = new List<string>();
            if (filters != null)
            {
                var filterStr = string.Join(" or ", filters.Select(s =>
                    $"{nameof(Application.Entities.Search.Job.JobType)} eq '{s.GetDescription()}'"));
                if (!string.IsNullOrWhiteSpace(filterStr))
                {
                   // filterStr = $"({filterStr})";
                    filterQuery.Add($"({filterStr})");
                }
            }

            if (location?.Point != null)
            {
                filterQuery.Add(
                    $"geo.distance({nameof(Application.Entities.Search.Job.Location)}, geography'POINT({location.Point.Longitude} {location.Point.Latitude})') le {JobSearch.RadiusOfFindingJobKm}");
            }

            return filterQuery;
        }
    }
}
