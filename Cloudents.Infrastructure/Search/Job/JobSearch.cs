using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entity = Cloudents.Core.Entities.Search;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Write;
using Cloudents.Infrastructure.Write.Job;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    public class JobSearch : IJobSearch
    {
        private readonly ISearchIndexClient _client;
        private readonly IMapper _mapper;

        private const int PageSize = 30;

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
            GeoPoint location,
            int page,
            CancellationToken token)
        {
            var filterQuery = new List<string>();
            var sortQuery = new List<string>();
            if (filter == JobRequestFilter.Paid)
            {
                filterQuery.Add("compensationType eq 'paid'");
            }

            if (jobType != null)
            {
                filterQuery.AddRange(jobType.Select(s => $"jobType eq '{s}'"));
                //filterQuery = string.Join(" or ", jobType.Select(s => $"jobType eq '{s}'"));
            }

            switch (sort)
            {
                case JobRequestSort.Distance when location != null:
                    sortQuery.Add($"geo.distance(location, geography'POINT({location.Longitude} {location.Latitude})')");
                    break;
                case JobRequestSort.Date:
                    sortQuery.Add("dateTime desc");
                    break;
            }
            var searchParams = new SearchParameters
            {
                Select = new[]
                {
                    nameof(Entity.Job.Title).CamelCase(),
                    nameof(Entity.Job.Description).CamelCase(),
                    nameof(Entity.Job.DateTime).CamelCase(),
                    nameof(Entity.Job.City).CamelCase(),
                    nameof(Entity.Job.State).CamelCase(),
                    nameof(Entity.Job.JobType).CamelCase(),
                    nameof(Entity.Job.Compensation).CamelCase(),
                    nameof(Entity.Job.Url).CamelCase(),
                    nameof(Entity.Job.Company).CamelCase(),


                },
                Facets = filterQuery.Count == 0 ? new[]
                {
                    nameof(Entity.Job.JobType).CamelCase()
                } : null,
                Top = PageSize,
                Skip = PageSize * page,
                Filter = string.Join(" or " , filterQuery),
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
                opt => opt.Items[JobResultConverter.FacetType] = nameof(Entity.Job.JobType).CamelCase());
        }
    }
}
