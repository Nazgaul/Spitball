using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Job
{
    /// <summary>
    /// Career jet builder
    /// Taken from https://www.careerjet.com/partners/api/php/ - source code
    /// </summary>
    public class CareerJetProvider : IJobProvider
    {
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public CareerJetProvider(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(string term, JobRequestSort sort, IEnumerable<JobFilter> jobType, Location location, int page, bool highlight,
            CancellationToken token)
        {
            var sortStr = string.Empty;
            switch (sort)
            {
                case JobRequestSort.Date:
                    sortStr = "date";
                    break;

            }

            var contactType = new List<string>();
            var contactPeriod = new List<string>();
            foreach (var filter in jobType ?? Enumerable.Empty<JobFilter>())
            {
                switch (filter)
                {
                    case JobFilter.None:
                        break;
                    case JobFilter.FullTime:
                        contactPeriod.Add("f");
                        break;
                    case JobFilter.PartTime:
                        contactPeriod.Add("p");
                        break;
                    case JobFilter.Contractor:
                        contactType.Add("c");
                        break;
                    case JobFilter.Temporary:
                        contactType.Add("t");
                        break;
                }
            }

            var nvc = new NameValueCollection
            {
                ["location"] = location != null ? $"{location.City}, {location.RegionCode}" : string.Empty,
                ["affid"] = "c307482e201e09643098fc2b06192f68",
                ["keywords"] = term,
                ["locale_code"] = "en_US",
                ["pagesize"] = JobSearch.PageSize.ToString(),
                ["page"] = page.ToString(),
                ["sort"] = sortStr,
                ["contracttype"] = string.Join(",", contactType),
                ["contractperiod"] = string.Join(",", contactPeriod)
            };

            var result = await _client.GetAsync(new Uri("http://public.api.careerjet.net/search"), nvc, token).ConfigureAwait(false);

            var p = JsonConvert.DeserializeObject<CareerJetResult>(result);
            var jobs = _mapper.Map<IEnumerable<JobDto>>(p);

            return new ResultWithFacetDto<JobDto>
            {
                Result = jobs,
                Facet = new[]
                {
                    JobFilter.FullTime.GetDescription(),
                    JobFilter.PartTime.GetDescription(),
                    JobFilter.Contractor.GetDescription(),
                    JobFilter.Temporary.GetDescription()
                }
            };
        }


        public class CareerJetResult
        {
            public Job[] jobs { get; set; }
            //public int hits { get; set; }
            //public float response_time { get; set; }
            //public string type { get; set; }
            //public int pages { get; set; }
        }

        public class Job
        {
            public string locations { get; set; }
           // public string site { get; set; }
            public DateTime date { get; set; }
            public string url { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string company { get; set; }
            //public string salary { get; set; }
            //public string salary_min { get; set; }
            //public string salary_type { get; set; }
            //public string salary_currency_code { get; set; }
            //public string salary_max { get; set; }
        }

    }
}
