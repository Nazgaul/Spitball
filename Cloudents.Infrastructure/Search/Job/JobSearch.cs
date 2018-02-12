using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Spatial;

namespace Cloudents.Infrastructure.Search.Job
{
    public class JobSearch : IJobSearch
    {
        private readonly IEnumerable<IJobProvider> _providers;
        //private readonly IUrlRedirectBuilder _urlRedirectBuilder;

        public const int PageSize = 30;

        public JobSearch(IEnumerable<IJobProvider> providers)
        {
            _providers = providers;
        }

        [BuildLocalUrl(nameof(ResultWithFacetDto<JobDto>.Result), PageSize, "page")]
        [Cache(TimeConst.Hour, "job")]
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
                var point = GeographyPoint.Create(location.Point.Latitude, location.Point.Latitude);
                jobResults = jobResults.OrderBy(o => point.Distance(o.Location));
            }
            return new ResultWithFacetDto<JobDto>
            {
                Result = jobResults,
                Facet = facets
            };
        }
    }
}
