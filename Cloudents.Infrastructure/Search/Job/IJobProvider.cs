using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    public class JobProviderRequest
    {
        public JobProviderRequest(string term, JobRequestSort sort,
            IEnumerable<JobFilter> jobType, Location location, int page
            )
        {
            Term = term;
            Sort = sort;
            JobType = jobType;
            Location = location;
            Page = page;
        }

        public string Term { get; private set; }
        public JobRequestSort Sort { get; private set; }
        public IEnumerable<JobFilter> JobType { get; private set; }
        public Location Location { get; private set; }
        public int Page { get; private set; }
    }

    public interface IJobProvider
    {
        Task<ResultWithFacetDto<JobDto>> SearchAsync(JobProviderRequest jobProviderRequest, CancellationToken token);
    }
}