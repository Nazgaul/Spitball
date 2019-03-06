﻿//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core;
//using Cloudents.Core.Attributes;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Extension;
//using Cloudents.Core.Interfaces;
//using JetBrains.Annotations;
//using Newtonsoft.Json;

//namespace Cloudents.Infrastructure.Search.Job
//{
//    /// <summary>
//    /// Career jet builder
//    /// Taken from https://www.careerjet.com/partners/api/php/ - source code
//    /// </summary>
//    [UsedImplicitly]
//    public class CareerJetProvider : IJobProvider
//    {
//        private readonly IRestClient _client;

//        public CareerJetProvider(IRestClient client)
//        {
//            _client = client;
//        }

//        [Cache(TimeConst.Hour, "job-careerJet", false)]
//        public async Task<ResultWithFacetDto<JobProviderDto>> SearchAsync(JobProviderRequest jobProviderRequest,CancellationToken token)
//        {
//            var contactType = new List<string>();
//            var contactPeriod = new List<string>();

//            var noResult = true;
//            if (jobProviderRequest.JobType == null)
//            {
//                noResult = false;
//            }
//            else
//            {
//                foreach (var filter in jobProviderRequest.JobType)
//                {
//                    switch (filter)
//                    {
//                        case JobFilter.None:
//                            noResult = false;
//                            break;
//                        case JobFilter.FullTime:
//                            contactPeriod.Add("f");
//                            noResult = false;
//                            break;
//                        case JobFilter.PartTime:
//                            contactPeriod.Add("p");
//                            noResult = false;
//                            break;
//                        case JobFilter.Contractor:
//                            contactType.Add("c");
//                            noResult = false;
//                            break;
//                        case JobFilter.Temporary:
//                            contactType.Add("t");
//                            noResult = false;
//                            break;
//                    }
//                }
//            }

//            if (noResult)
//            {
//                return null;
//            }

//            var nvc = new NameValueCollection
//            {
//                ["affid"] = "c307482e201e09643098fc2b06192f68",
//                ["keywords"] = jobProviderRequest.Term,
//                ["locale_code"] = "en_US",
//                ["pagesize"] = JobSearch.PageSize.ToString(),
//                ["page"] = jobProviderRequest.Page.ToString(),
//                ["sort"] = jobProviderRequest.Sort == JobRequestSort.Relevance ? "relevance" : "date",
//                ["contracttype"] = string.Join(",", contactType),
//                ["contractperiod"] = string.Join(",", contactPeriod),
//            };
//            if (/*sort == JobRequestSort.Distance &&*/ jobProviderRequest.Location?.Address != null)
//            {
//                nvc.Add("location", $"{jobProviderRequest.Location.Address.City}, {jobProviderRequest.Location.Address.RegionCode}");
//            }

//            var result = await _client.GetAsync<CareerJetResult>(new Uri("http://public.api.careerjet.net/search"), nvc, token);
//            if (result == null)
//            {
//                return null;
//            }

//            if (result.Hits == 0)
//            {
//                return null;
//            }

//            var jobs = result.Jobs.Select((s, i) => new JobProviderDto
//            {
//                DateTime = s.Date,
//                Url = s.Url,
//                PrioritySource = PrioritySource.JobCareerJet,
//                Address = s.Locations,
//                Title = s.Title,
//                Company = s.Company,
//                CompensationType = "Paid",
//                Responsibilities = s.Description.StripAndDecode(), Order = i + 1
//            });

//            return new ResultWithFacetDto<JobProviderDto>
//            {
//                Result = jobs,
//                Facet = new[]
//                {
//                    JobFilter.FullTime.GetDescription(),
//                    JobFilter.PartTime.GetDescription(),
//                    JobFilter.Contractor.GetDescription(),
//                    JobFilter.Temporary.GetDescription()
//                }
//            };
//        }

//        public class CareerJetResult
//        {
//            [JsonProperty("hits")]
//            public int Hits { get; set; }

//            [JsonProperty("jobs")]
//            public Job[] Jobs { get; set; }
//            //public float response_time { get; set; }
//            //public string type { get; set; }
//            //public int pages { get; set; }
//        }

//        public class Job
//        {
//            [JsonProperty("locations")]
//            public string Locations { get; set; }
//            // public string site { get; set; }
//            [JsonProperty("date")]
//            public DateTime Date { get; set; }

//            [JsonProperty("url")]
//            public string Url { get; set; }

//            [JsonProperty("title")]
//            public string Title { get; set; }

//            [JsonProperty("description")]
//            public string Description { get; set; }

//            [JsonProperty("company")]
//            public string Company { get; set; }
//            //public string salary { get; set; }
//            //public string salary_min { get; set; }
//            //public string salary_type { get; set; }
//            //public string salary_currency_code { get; set; }
//            //public string salary_max { get; set; }
//        }
//    }
//}
