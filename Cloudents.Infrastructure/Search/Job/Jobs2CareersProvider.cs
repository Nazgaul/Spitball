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
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Job
{
    /// <summary>
    /// <remarks>http://api.jobs2careers.com/api/spec.pdf</remarks>
    /// </summary>
    [UsedImplicitly]
    public class Jobs2CareersProvider : IJobProvider
    {
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public Jobs2CareersProvider(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        [Cache(TimeConst.Hour, nameof(Jobs2CareersProvider), false)]
        public async Task<ResultWithFacetDto<JobProviderDto>> SearchAsync(JobProviderRequest jobProviderRequest, CancellationToken token)
        {
            if (jobProviderRequest.Location?.Address?.City == null || jobProviderRequest.Location.Ip == null)
            {
                return null;
            }

            if (!string.Equals(jobProviderRequest.Location.Address.CountryCode, "us", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var nvc = new NameValueCollection
            {
                ["id"] = 4538.ToString(),
                ["pass"] = "PGhLlpYJEEeZjxCd",
                ["ip"] = jobProviderRequest.Location.Ip,
                ["l"] = jobProviderRequest.Location.Address.City,
                ["start"] = (jobProviderRequest.Page * JobSearch.PageSize).ToString(),
                ["Limit"] = JobSearch.PageSize.ToString(),
                ["format"] = "json",
                ["link"] = 1.ToString(),
                ["q"] = jobProviderRequest.Term,
                ["sort"] = jobProviderRequest.Sort == JobRequestSort.Date ? "d" : "r",
                ["d"] = JobSearch.RadiusOfFindingJobKm.ToString(CultureInfo.InvariantCulture)
            };

            var jobFilter = new List<string>();
            foreach (var filter in jobProviderRequest.JobType ?? Enumerable.Empty<JobFilter>())
            {
                switch (filter)
                {
                    case JobFilter.FullTime:
                        jobFilter.Add("1");
                        break;
                    case JobFilter.PartTime:
                        jobFilter.Add("2");
                        break;
                    case JobFilter.Temporary:
                        jobFilter.Add("4");
                        break;
                }
            }
            nvc.Add("jobtype", string.Join(",", jobFilter));

            var result = await _client.GetAsync<Jobs2CareersResult>(new Uri("http://api.jobs2careers.com/api/search.php"), nvc, token).ConfigureAwait(false);

            if (result == null)
            {
                return null;
            }
            if (result.Total == 0)
            {
                return null;
            }

            var jobs = _mapper.MapWithPriority<Job, JobProviderDto>(result.Jobs);

            return new ResultWithFacetDto<JobProviderDto>
            {
                Result = jobs,
                Facet = new[]
                {
                    JobFilter.FullTime.GetDescription(),
                    JobFilter.PartTime.GetDescription(),
                    JobFilter.Temporary.GetDescription(),
                }
            };
        }

        public class Jobs2CareersResult
        {
            [JsonProperty("jobs")]
            public Job[] Jobs { get; set; }
            [JsonProperty("total")]
            public int Total { get; set; }
            //public int start { get; set; }
            //public int count { get; set; }
        }

        public class Job
        {
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("date")]
            public DateTime Date { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("company")]
            public string Company { get; set; }
            [JsonProperty("city")]
            public string[] City { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
            //public string major_category0 { get; set; }
            //public string minor_category0 { get; set; }
            //public object[] zipcode { get; set; }
            //public string price { get; set; }
            //public int preview { get; set; }

        }
    }
}
