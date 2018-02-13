using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
    public class IndeedProvider : IJobProvider
    {
        //https://ads.indeed.com/jobroll/xmlfeed
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public IndeedProvider(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(string term, JobRequestSort sort, IEnumerable<JobFilter> jobType, Location location, int page, bool highlight,
            CancellationToken token)
        {
            var locationStr = string.Empty;
            if (location != null)
            {
                locationStr = $"{location.City}, {location.RegionCode}";
            }

            //var sortStr = string.Empty;
            //switch (sort)
            //{
            //    case JobRequestSort.Date:
            //        sortStr = "date";
            //        break;
            //}

            var jobFilter = new List<string>();
            foreach (var filter in jobType ?? Enumerable.Empty<JobFilter>())
            {
                switch (filter)
                {
                    case JobFilter.FullTime:
                        jobFilter.Add("fulltime");
                        break;
                    case JobFilter.PartTime:
                        jobFilter.Add("parttime");
                        break;
                    case JobFilter.Internship:
                        jobFilter.Add("internship");
                        break;
                    case JobFilter.Contractor:
                        jobFilter.Add("contract");
                        break;
                    case JobFilter.Temporary:
                        jobFilter.Add("temporary");
                        break;
                    case JobFilter.None:
                        break;
                    case JobFilter.CampusRep:
                        break;
                    case JobFilter.Remote:
                        break;
                }
            }

            var nvc = new NameValueCollection
            {
                ["v"] = 2.ToString(),
                ["format"] = "json",
                ["publisher"] = 5421359041330050.ToString(),
                ["q"] = term,
                ["sort"] = sort == JobRequestSort.Date ? "date" : string.Empty,
                ["l"] = locationStr,
                ["limit"] = JobSearch.PageSize.ToString(),
                ["start"] = (page * JobSearch.PageSize).ToString(),
                ["highlight"] = 0.ToString(),
                ["jt"] = string.Join(",", jobFilter),
                //["latlong"] = 1.ToString()
            };

            if (sort == JobRequestSort.Date)
            {
                nvc.Add("sort", "date");
            }
            else
            {
                nvc.Add("radius", JobSearch.RadiusOfFindingJob.ToString(CultureInfo.InvariantCulture));
            }
            var result = await _client.GetAsync(new Uri("http://api.indeed.com/ads/apisearch"), nvc, token).ConfigureAwait(false);

            var p = JsonConvert.DeserializeObject<IndeedResult>(result);
            var jobs = _mapper.Map<IEnumerable<JobDto>>(p);

            return new ResultWithFacetDto<JobDto>
            {
                Result = jobs,
                Facet = new[]
                {
                    JobFilter.FullTime.GetDescription(),
                    JobFilter.PartTime.GetDescription(),
                    JobFilter.Contractor.GetDescription(),
                    JobFilter.Internship.GetDescription(),
                    JobFilter.Temporary.GetDescription()
                }
            };
        }


        public class IndeedResult
        {
            // public int version { get; set; }
            // public string query { get; set; }
            // public string location { get; set; }
            //public string paginationPayload { get; set; }
            // public bool dupefilter { get; set; }
            // public bool highlight { get; set; }
            // public int totalResults { get; set; }
            //public int start { get; set; }
            //public int end { get; set; }
            //public int pageNumber { get; set; }
            [JsonProperty("results")]
            public Result[] Results { get; set; }
        }

        public class Result
        {
            [JsonProperty("jobtitle")]
            public string JobTitle { get; set; }
            [JsonProperty("company")]
            public string Company { get; set; }
            // public string city { get; set; }
            // public string state { get; set; }
            // public string country { get; set; }
            // public string language { get; set; }
            [JsonProperty("formattedLocation")]
            public string FormattedLocation { get; set; }
            //public string source { get; set; }
            [JsonProperty("date")]
            public DateTime Date { get; set; }
            [JsonProperty("snippet")]
            public string Snippet { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            //public string onmousedown { get; set; }
            [JsonProperty("latitude")]
            public float Latitude { get; set; }
            [JsonProperty("longitude")]
            public float Longitude { get; set; }
            //public string jobkey { get; set; }
            // public bool sponsored { get; set; }
            //public bool expired { get; set; }
            //public bool indeedApply { get; set; }
            // public string formattedLocationFull { get; set; }
            //public string formattedRelativeTime { get; set; }
            // public string stations { get; set; }
        }

    }
}
