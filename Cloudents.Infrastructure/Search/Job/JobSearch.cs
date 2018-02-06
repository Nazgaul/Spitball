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
        private readonly IUrlRedirectBuilder<JobDto> _urlRedirectBuilder;

        public const int PageSize = 30;

        public JobSearch(IEnumerable<IJobProvider> providers, IUrlRedirectBuilder<JobDto> urlRedirectBuilder)
        {
            _providers = providers;
            _urlRedirectBuilder = urlRedirectBuilder;
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(IEnumerable<string> term, JobRequestSort sort, IEnumerable<string> jobType, Location location,
            int page, bool highlight, CancellationToken token)
        {
            var str = string.Join(" ", term ?? Enumerable.Empty<string>());
            var tasks = _providers.Select(s => s.SearchAsync(str.Trim(), sort, jobType, location, page, highlight, token)).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = tasks.Select(s => s.Result).Where(w => w != null).ToList();
            var facets = result.Where(w => w.Facet != null).SelectMany(s => s.Facet).Distinct();
            var jobs = Shuffle<JobDto>.DoShuffle(result.SelectMany(s => s.Result));
            jobs = _urlRedirectBuilder.BuildUrl(page, PageSize, jobs);
            return new ResultWithFacetDto<JobDto>
            {
                Result = jobs,
                Facet = facets
            };
        }
    }
}
