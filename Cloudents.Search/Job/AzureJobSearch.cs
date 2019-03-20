//using Cloudents.Core;
//using Cloudents.Core.Attributes;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Extension;
//using Cloudents.Core.Models;
//using Cloudents.Infrastructure.Search.Job;
//using JetBrains.Annotations;
//using Microsoft.Azure.Search;
//using Microsoft.Azure.Search.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Search.Job
//{
//    [UsedImplicitly]
//    public class AzureJobSearch : IJobProvider
//    {
//        private readonly ISearchIndexClient _client;

//        public AzureJobSearch(ISearchService client)
//        {
//            _client = client.GetOldClient(JobSearchWrite.IndexName);
//        }

//        [Cache(TimeConst.Hour, "job-azure", false)]
//        [Log]
//        public async Task<ResultWithFacetDto<JobProviderDto>> SearchAsync(JobProviderRequest jobProviderRequest, CancellationToken token)
//        {
//            var sortQuery = new List<string>();

//            var filterQuery = BuildFilter(jobProviderRequest.JobType, jobProviderRequest.Location);

//            if (jobProviderRequest.Sort == JobRequestSort.Date)
//            {
//                sortQuery.Add($"{nameof(Entities.Job.DateTime)} desc");
//            }
//            var searchParams = new SearchParameters
//            {
//                Select = new[]
//                {
//                    nameof(Entities.Job.Title),
//                    nameof(Entities.Job.Description),
//                    nameof(Entities.Job.DateTime),
//                    nameof(Entities.Job.City),
//                    nameof(Entities.Job.State),
//                    nameof(Entities.Job.JobType),
//                    nameof(Entities.Job.Url),
//                    nameof(Entities.Job.Company),
//                    nameof(Entities.Job.Source)
//                },
//                Facets = filterQuery.Count == 0 ? new[]
//                {
//                    nameof(Entities.Job.JobType)
//                } : null,
//                Top = JobSearch.PageSize,
//                Skip = JobSearch.PageSize * jobProviderRequest.Page,
//                Filter = string.Join(" and ", filterQuery),
//                OrderBy = sortQuery
//            };

//            var retValSearchResult = await
//                _client.Documents.SearchAsync<Entities.Job>(jobProviderRequest.Term, searchParams, cancellationToken: token);
//            if (retValSearchResult.Results.Count == 0)
//            {
//                return null;
//            }


//            var retVal = new ResultWithFacetDto<JobProviderDto>
//            {

//                Result = retValSearchResult.Results.Select((s, i) => new JobProviderDto()
//                {
//                    Url = s.Document.Url,
//                    CompensationType = "Paid",
//                    Company = s.Document.Company,
//                    DateTime = s.Document.DateTime.GetValueOrDefault(),
//                    Address = $"{s.Document.City}, {s.Document.State}",
//                    Title = s.Document.Title,
//                    Responsibilities = s.Document.Description,
//                    PrioritySource = PrioritySource.JobWayUp,
//                    Order = i + 1
//                })
//            };

//            if (retValSearchResult.Facets != null)
//            {
//                retValSearchResult.Facets.TryGetValue(nameof(Entities.Job.JobType), out var facets);
//                retVal.Facet = facets?.Select(s => s.AsValueFacetResult<string>().Value).Where(w => !string.Equals(w,
//                    "none", StringComparison.OrdinalIgnoreCase));
//            }

//            return retVal;
//            //return _mapper.Map<ResultWithFacetDto<JobProviderDto>>(retVal);
//        }

//        private static IList<string> BuildFilter(IEnumerable<JobFilter> filters, Location location)
//        {
//            var filterQuery = new List<string>();
//            if (filters != null)
//            {
//                var filterStr = string.Join(" or ", filters.Select(s =>
//                    $"{nameof(Entities.Job.JobType)} eq '{s.GetDescription()}'"));
//                if (!string.IsNullOrWhiteSpace(filterStr))
//                {
//                    // filterStr = $"({filterStr})";
//                    filterQuery.Add($"({filterStr})");
//                }
//            }

//            if (location?.Point != null)
//            {
//                filterQuery.Add(
//                    $"geo.distance({nameof(Entities.Job.Location)}, geography'POINT({location.Point.Longitude} {location.Point.Latitude})') le {JobSearch.RadiusOfFindingJobKm}");
//            }

//            return filterQuery;
//        }
//    }
//}
