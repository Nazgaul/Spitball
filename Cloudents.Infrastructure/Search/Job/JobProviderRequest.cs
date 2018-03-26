using System.Collections.Generic;
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

        public string Term { get; }
        public JobRequestSort Sort { get; }
        public IEnumerable<JobFilter> JobType { get; }
        public Location Location { get; }
        public int Page { get; }
    }
}