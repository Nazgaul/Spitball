using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Job
{
    /// <summary>
    /// <remarks>https://docs.google.com/document/d/1PM6pgV06vGbQJoZ1mxeSTZPr1aYemSGoEyTtNTDyt3s/pub</remarks>
    /// </summary>
    [UsedImplicitly]
    public class ZipRecruiterClient : IJobProvider
    {
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public ZipRecruiterClient(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        [Cache(TimeConst.Hour, "job-zipRecruiter", false)]
        public async Task<ResultWithFacetDto<JobProviderDto>> SearchAsync(JobProviderRequest jobProviderRequest,CancellationToken token)
        {
            if (jobProviderRequest.JobType?.Any() == true 
                || jobProviderRequest.Location?.Address == null)
            {
                return null;
            }

            if (string.Equals(jobProviderRequest.Location.Address.CountryCode, "us", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            var nvc = new NameValueCollection
            {
                ["location"] = $"{jobProviderRequest.Location.Address.City}, {jobProviderRequest.Location.Address.RegionCode}",
                ["api_key"] = "x8w8rgmv2dq78dw5wfmwiwexwu3hdfv3",
                ["search"] = jobProviderRequest.Term,
                ["jobs_per_page"] = JobSearch.PageSize.ToString(),
                ["page"] = jobProviderRequest.Page.ToString(),
                ["radius_miles"] = JobSearch.RadiusOfFindingJobMiles.ToString(CultureInfo.InvariantCulture)
            };

            var result = await _client.GetAsync<ZipRecruiterResult>(new Uri("https://api.ziprecruiter.com/jobs/v1"), nvc, token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }

            if (!result.Success)
            {
                return null;
            }

            var jobs = _mapper.MapWithPriority<Job, JobProviderDto>(result.Jobs);

            return new ResultWithFacetDto<JobProviderDto>
            {
                Result = jobs
            };
        }

        public class ZipRecruiterResult
        {
            [JsonProperty("jobs")]
            public Job[] Jobs { get; set; }
            [JsonProperty("success")]
            public bool Success { get; set; }
        }

        public class Job
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("snippet")]
            public string Snippet { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("posted_time")]
            public DateTime PostedTime { get; set; }
            [JsonProperty("hiring_company")]
            public HiringCompany HiringCompany { get; set; }
            [JsonProperty("location")]
            public string Location { get; set; }
        }

        public class HiringCompany
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
