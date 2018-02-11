using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Job
{
    public class ZipRecruiterClient : IJobProvider
    {
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public ZipRecruiterClient(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(string term,
            JobRequestSort sort, IEnumerable<JobFilter> jobType, Location location,
            int page, bool highlight, CancellationToken token)
        {
            if (jobType?.Any() == true)
            {
                return null;
            }

            if (location == null)
            {
                return null;
            }
            var nvc = new NameValueCollection
            {
                ["location"] = $"{location.City}, {location.RegionCode}",
                ["api_key"] = "x8w8rgmv2dq78dw5wfmwiwexwu3hdfv3",
                ["search"] = term,
                ["jobs_per_page"] = JobSearch.PageSize.ToString(),
                ["page"] = page.ToString()
            };
            var result = await _client.GetAsync(new Uri("https://api.ziprecruiter.com/jobs/v1"), nvc, token).ConfigureAwait(false);

            var p = JsonConvert.DeserializeObject<ZipRecruiterResult>(result,
                new JsonSerializerSettings
                {
                    ContractResolver = new UnderscorePropertyNamesContractResolver()
                });
            var jobs = _mapper.Map<IEnumerable<JobDto>>(p);

            if (sort == JobRequestSort.Date)
            {
                jobs = jobs.OrderByDescending(o => o.DateTime);
            }

            return new ResultWithFacetDto<JobDto>
            {
                Result = jobs
            };
        }

        public class ZipRecruiterResult
        {
            public Job[] Jobs { get; set; }
            public bool Success { get; set; }
        }

        public class Job
        {
            public string Url { get; set; }
            public string Snippet { get; set; }
            public string Name { get; set; }
            public DateTime PostedTime { get; set; }
            public HiringCompany HiringCompany { get; set; }
            public string Location { get; set; }
        }

        public class HiringCompany
        {
            public string Name { get; set; }
        }
    }
}
