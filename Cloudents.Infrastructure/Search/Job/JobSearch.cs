﻿using System.Collections.Generic;
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
            int page, bool highlight, CancellationToken token)
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
            var jobRequest = new JobProviderRequest(str.Trim(), sort, facetEnum, location, page);
            var tasks = _providers.Select(s => s.SearchAsync(jobRequest, token));
            var tasksResult = await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = tasksResult.Where(w => w != null).ToList();
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
