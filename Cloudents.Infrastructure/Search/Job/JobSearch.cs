using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    public class JobSearch : IJobSearch
    {
        private readonly IEnumerable<IJobProvider> _providers;
        private readonly IShuffle _shuffle;

        public const int PageSize = 30;
        public const double RadiusOfFindingJobKm = RadiusOfFindingJobMiles * 1.6;
        public const double RadiusOfFindingJobMiles = 50 ;

        public JobSearch(IEnumerable<IJobProvider> providers, IShuffle shuffle)
        {
            _providers = providers;
            _shuffle = shuffle;
        }

        [BuildLocalUrl(nameof(ResultWithFacetDto<JobDto>.Result), PageSize, "page")]
        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(IEnumerable<string> term, JobRequestSort sort, IEnumerable<JobFilter> jobType, Location location,
            int page, bool highlight, CancellationToken token)
        {
            var str = string.Join(" ", term ?? Enumerable.Empty<string>());
            var tasks = _providers.Select(s => s.SearchAsync(str.Trim(), sort, jobType, location, page, highlight, token)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = tasks.Select(s => s.Result).Where(w => w != null).ToList();
            var facets = result.Where(w => w.Facet != null).SelectMany(s => s.Facet).Distinct();
            var jobResults = result.Where(w => w.Result != null).SelectMany(s => s.Result);

            if (sort == JobRequestSort.Date)
            {
                jobResults = jobResults.OrderByDescending(o => o.DateTime);
            }
            else
            {
                jobResults = _shuffle.DoShuffle(jobResults);
            }
            return new ResultWithFacetDto<JobDto>
            {
                Result = jobResults,
                Facet = facets
            };
        }
    }
}
