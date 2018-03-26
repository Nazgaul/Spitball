using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Job
{
    [UsedImplicitly]
    public class ZipRecruiterClient : IJobProvider
    {
        //https://docs.google.com/document/d/1PM6pgV06vGbQJoZ1mxeSTZPr1aYemSGoEyTtNTDyt3s/pub
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public ZipRecruiterClient(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }


        [Cache(TimeConst.Hour, "job-zipRecruiter", false)]
        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(JobProviderRequest jobProviderRequest,CancellationToken token)
        {
            if (jobProviderRequest.JobType?.Any() == true 
                || jobProviderRequest.Location?.Address == null/* || sort == JobRequestSort.Distance*/)
            {
                return null;
            }
            var nvc = new NameValueCollection
            {
                ["location"] = $"{jobProviderRequest.Location.Address.City}, {jobProviderRequest.Location.Address.RegionCode}",
                ["api_key"] = "x8w8rgmv2dq78dw5wfmwiwexwu3hdfv3",
                ["search"] = jobProviderRequest.Term,
                ["jobs_per_page"] = JobSearch.PageSize.ToString(),
                ["page"] = jobProviderRequest.Page.ToString()
            };

            //if (sort == JobRequestSort.Distance)
            //{
                nvc.Add("radius_miles", JobSearch.RadiusOfFindingJobMiles.ToString(CultureInfo.InvariantCulture));
            //}

            var result = await _client.GetAsync(new Uri("https://api.ziprecruiter.com/jobs/v1"), nvc, token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            var p = JsonConvert.DeserializeObject<ZipRecruiterResult>(result);
            var jobs = _mapper.Map<IEnumerable<JobDto>>(p);

            //if (sort == JobRequestSort.Date)
            //{
            //    jobs = jobs.OrderByDescending(o => o.DateTime);
            //}

            return new ResultWithFacetDto<JobDto>
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
