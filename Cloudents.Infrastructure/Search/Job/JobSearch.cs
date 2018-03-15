using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Job
{
    [UsedImplicitly]
    public class JobSearch : IJobSearch
    {
        private readonly IEnumerable<IJobProvider> _providers;
        private readonly IShuffle _shuffle;

        public const int PageSize = 10;
        public const double RadiusOfFindingJobKm = RadiusOfFindingJobMiles * 1.6;
        public const double RadiusOfFindingJobMiles = 50;

        public JobSearch(IEnumerable<IJobProvider> providers, IShuffle shuffle)
        {
            _providers = providers;
            _shuffle = shuffle;
        }

        [BuildLocalUrl(nameof(ResultWithFacetDto<JobDto>.Result), PageSize, "page")]
        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(IEnumerable<string> term, JobRequestSort sort, IEnumerable<string> jobType, Location location,
            int page, CancellationToken token)
        {
            var str = string.Join(" ", term ?? Enumerable.Empty<string>());

            var facetEnum = jobType?.Select(s =>
            {
                if (s.TryToEnum(out JobFilter p))
                {
                    return p;
                }
                return JobFilter.None;
            }).Where(w => w != JobFilter.None);
            var tasks = _providers.Select(s => s.SearchAsync(str.Trim(), sort, facetEnum, location, page, token)).ToList();
            var tasksResult = await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = tasksResult.Where(w => w != null).ToList();

            var retVal = result.Aggregate((dto, next) =>
            {
                if (next.Result != null)
                {
                    (dto.Result ?? Enumerable.Empty<JobDto>()).Union(next.Result);
                }

                if (next.Facet != null)
                {
                    (dto.Facet ?? Enumerable.Empty<string>()).Union(next.Facet);
                }

                return dto;
            });
            //var facets = result.Where(w => w.Facet != null).SelectMany(s => s.Facet).Distinct();
            //var jobResults = result.Where(w => w.Result != null).SelectMany(s => s.Result);
            retVal.Facet = retVal.Facet?.Distinct();
            if (sort == JobRequestSort.Date)
            {
                retVal.Result = retVal.Result.OrderByDescending(o => o.DateTime);
                //jobResults = jobResults.OrderByDescending(o => o.DateTime);
            }
            else
            {
                retVal.Result = _shuffle.DoShuffle(retVal.Result);
                //jobResults = _shuffle.DoShuffle(jobResults);
            }

            return retVal;
            //return new ResultWithFacetDto<JobDto>
            //{
            //    Result = jobResults,
            //    Facet = facets
            //};
        }
    }
}
