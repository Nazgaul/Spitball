﻿//using Cloudents.Core;
//using Cloudents.Core.Attributes;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Extension;
//using Cloudents.Core.Interfaces;
//using JetBrains.Annotations;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Globalization;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Infrastructure.Search.Job
//{
//    [UsedImplicitly]
//    public class IndeedProvider : IJobProvider
//    {
//        //https://ads.indeed.com/jobroll/xmlfeed
//        private readonly IRestClient _client;

//        public IndeedProvider(IRestClient client)
//        {
//            _client = client;
//        }

//        [Cache(TimeConst.Hour, "job-indeed", false)]
//        public async Task<ResultWithFacetDto<JobProviderDto>> SearchAsync(JobProviderRequest jobProviderRequest, CancellationToken token)
//        {
//            var locationStr = string.Empty;
//            if (jobProviderRequest.Location?.Address != null)
//            {
//                locationStr = $"{jobProviderRequest.Location.Address.City}, {jobProviderRequest.Location.Address.RegionCode}";
//            }

//            var jobFilter = new List<string>();
//            foreach (var filter in jobProviderRequest.JobType ?? Enumerable.Empty<JobFilter>())
//            {
//                switch (filter)
//                {
//                    case JobFilter.FullTime:
//                        jobFilter.Add("fulltime");
//                        break;
//                    case JobFilter.PartTime:
//                        jobFilter.Add("parttime");
//                        break;
//                    case JobFilter.Internship:
//                        jobFilter.Add("internship");
//                        break;
//                    case JobFilter.Contractor:
//                        jobFilter.Add("contract");
//                        break;
//                    case JobFilter.Temporary:
//                        jobFilter.Add("temporary");
//                        break;
//                    case JobFilter.None:
//                        break;
//                    case JobFilter.CampusRep:
//                        break;
//                    case JobFilter.Remote:
//                        break;
//                }
//            }

//            var nvc = new NameValueCollection
//            {
//                ["v"] = 2.ToString(),
//                ["format"] = "json",
//                ["publisher"] = 5421359041330050.ToString(),
//                ["q"] = jobProviderRequest.Term,
//                ["sort"] = jobProviderRequest.Sort == JobRequestSort.Date ? "date" : string.Empty,
//                ["l"] = locationStr,
//                ["limit"] = JobSearch.PageSize.ToString(),
//                ["start"] = (jobProviderRequest.Page * JobSearch.PageSize).ToString(),
//                ["highlight"] = 0.ToString(),
//                ["jt"] = string.Join(",", jobFilter),
//                ["radius"] = JobSearch.RadiusOfFindingJobKm.ToString(CultureInfo.InvariantCulture)
//                //["latlong"] = 1.ToString()
//            };

//            if (jobProviderRequest.Sort == JobRequestSort.Date)
//            {
//                nvc.Add("sort", "date");
//            }

//            var result = await _client.GetAsync<IndeedResult>(new Uri("http://api.indeed.com/ads/apisearch"), nvc, token);
//            if (result == null)
//            {
//                return null;
//            }

//            if (result.TotalResults == 0)
//            {
//                return null;
//            }

//            var jobs = result.Results.Select((s, i) => new JobProviderDto
//            {
//                DateTime = s.Date,
//                Url = s.Url,
//                PrioritySource = PrioritySource.JobIndeed,
//                //Source = "Indeed",
//                Address = s.FormattedLocation,
//                Title = s.JobTitle,
//                Company = s.Company,
//                CompensationType = "Paid",
//                Responsibilities = s.Snippet,
//                Order = i + 1
//            });

//            return new ResultWithFacetDto<JobProviderDto>
//            {
//                Result = jobs,
//                Facet = new[]
//                {
//                    JobFilter.FullTime.GetDescription(),
//                    JobFilter.PartTime.GetDescription(),
//                    JobFilter.Contractor.GetDescription(),
//                    JobFilter.Internship.GetDescription(),
//                    JobFilter.Temporary.GetDescription()
//                }
//            };
//        }

//        public class IndeedResult
//        {
//            // public int version { get; set; }
//            // public string query { get; set; }
//            // public string location { get; set; }
//            //public string paginationPayload { get; set; }
//            // public bool dupefilter { get; set; }
//            // public bool highlight { get; set; }
//            [JsonProperty("totalResults")]
//            public int TotalResults { get; set; }
//            //public int start { get; set; }
//            //public int end { get; set; }
//            //public int pageNumber { get; set; }
//            [JsonProperty("results")]
//            public Result[] Results { get; set; }
//        }

//        public class Result
//        {
//            [JsonProperty("jobtitle")]
//            public string JobTitle { get; set; }

//            [JsonProperty("company")]
//            public string Company { get; set; }
//            // public string city { get; set; }
//            // public string state { get; set; }
//            // public string country { get; set; }
//            // public string language { get; set; }
//            [JsonProperty("formattedLocation")]
//            public string FormattedLocation { get; set; }
//            //public string source { get; set; }
//            [JsonProperty("date")]
//            public DateTime Date { get; set; }

//            [JsonProperty("snippet")]
//            public string Snippet { get; set; }

//            [JsonProperty("url")]
//            public string Url { get; set; }
//            //public string onmousedown { get; set; }
//            [JsonProperty("latitude")]
//            public float Latitude { get; set; }

//            [JsonProperty("longitude")]
//            public float Longitude { get; set; }
//            //public string jobkey { get; set; }
//            // public bool sponsored { get; set; }
//            //public bool expired { get; set; }
//            //public bool indeedApply { get; set; }
//            // public string formattedLocationFull { get; set; }
//            //public string formattedRelativeTime { get; set; }
//            // public string stations { get; set; }
//        }
//    }
//}
