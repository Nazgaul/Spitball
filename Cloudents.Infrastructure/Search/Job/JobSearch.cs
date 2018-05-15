using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;
using IMapper = AutoMapper.IMapper;

namespace Cloudents.Infrastructure.Search.Job
{
    [UsedImplicitly]
    public class JobSearch : IJobSearch
    {
        private readonly IEnumerable<IJobProvider> _providers;
        private readonly IShuffle _shuffle;
        private readonly IMapper _mapper;


        public const int PageSize = 10;
        public const double RadiusOfFindingJobKm = RadiusOfFindingJobMiles * 1.6;
        public const double RadiusOfFindingJobMiles = 50;

        public JobSearch(IEnumerable<IJobProvider> providers, IShuffle shuffle, IMapper mapper)
        {
            _providers = providers;
            _shuffle = shuffle;
            _mapper = mapper;
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
            var jobRequest = new JobProviderRequest(str.Trim(), sort, facetEnum, location, page);
            var tasks = _providers.Select(s => s.SearchAsync(jobRequest, token));
            var tasksResult = await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = tasksResult.Where(w => w != null).ToList();

            var retVal = result.Aggregate(new ResultWithFacetDto<JobDto>(), (dto,  next) =>
            {
                if (next.Result != null)
                {
                    var resultNext = _mapper.Map<IEnumerable<JobDto>>(next.Result);
                    dto.Result = (dto.Result ?? Enumerable.Empty<JobDto>()).Union(resultNext);
                }

                if (next.Facet != null)
                {
                    dto.Facet = (dto.Facet ?? Enumerable.Empty<string>()).Union(next.Facet);
                }

                return dto;
            });

            retVal.Facet = retVal.Facet?.Distinct();
            if (sort == JobRequestSort.Date)
            {
                retVal.Result = retVal.Result.OrderByDescending(o => o.DateTime);
            }
            else
            {
                retVal.Result = _shuffle.ShuffleByPriority(retVal.Result);
            }

            return retVal;
        }
    }
}
