using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Cloudents.Infrastructure.Extensions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Job
{
    /// <summary>
    /// Career jet builder
    /// Taken from https://www.careerjet.com/partners/api/php/ - source code
    /// </summary>
    [UsedImplicitly]
    public class CareerJetProvider : IJobProvider
    {
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public CareerJetProvider(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        [Cache(TimeConst.Hour, "job-careerJet", false)]
        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(string term, JobRequestSort sort, IEnumerable<JobFilter> jobType, Location location, int page, HighlightTextFormat highlight,
            CancellationToken token)
        {
            var contactType = new List<string>();
            var contactPeriod = new List<string>();

            var noResult = true;
            if (jobType == null)
            {
                noResult = false;
            }
            else
            {
                foreach (var filter in jobType)
                {
                    switch (filter)
                    {
                        case JobFilter.None:
                            noResult = false;
                            break;
                        case JobFilter.FullTime:
                            contactPeriod.Add("f");
                            noResult = false;
                            break;
                        case JobFilter.PartTime:
                            contactPeriod.Add("p");
                            noResult = false;
                            break;
                        case JobFilter.Contractor:
                            contactType.Add("c");
                            noResult = false;
                            break;
                        case JobFilter.Temporary:
                            contactType.Add("t");
                            noResult = false;
                            break;
                    }
                }
            }


            if (noResult)
            {
                return null;
            }

            var nvc = new NameValueCollection
            {
                ["affid"] = "c307482e201e09643098fc2b06192f68",
                ["keywords"] = term,
                ["locale_code"] = "en_US",
                ["pagesize"] = JobSearch.PageSize.ToString(),
                ["page"] = page.ToString(),
                ["sort"] = sort == JobRequestSort.Relevance ? "relevance" : "date",
                ["contracttype"] = string.Join(",", contactType),
                ["contractperiod"] = string.Join(",", contactPeriod),
            };
            if (location?.Address != null)
            {
                nvc.Add("location", $"{location.Address.City}, {location.Address.RegionCode}");
            }

            var result = await _client.GetAsync<CareerJetResult>(new Uri("http://public.api.careerjet.net/search"), nvc, token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }

            if (result.Hits == 0)
            {
                return null;
            }


            var jobs = _mapper.MapWithPriority<Job, JobDto>(result.Jobs);

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
            [JsonProperty("hits")]
            public int Hits { get; set; }
            [JsonProperty("jobs")]
            public Job[] Jobs { get; set; }
            //public float response_time { get; set; }
            //public string type { get; set; }
            //public int pages { get; set; }
        }

        public class Job
        {
            [JsonProperty("locations")]
            public string Locations { get; set; }
            // public string site { get; set; }
            [JsonProperty("date")]
            public DateTime Date { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("company")]
            public string Company { get; set; }
            //public string salary { get; set; }
            //public string salary_min { get; set; }
            //public string salary_type { get; set; }
            //public string salary_currency_code { get; set; }
            //public string salary_max { get; set; }
        }
    }
}
