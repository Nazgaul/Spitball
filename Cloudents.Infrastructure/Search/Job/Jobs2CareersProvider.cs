﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
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
    public class Jobs2CareersProvider : IJobProvider
    {
        //http://api.jobs2careers.com/api/spec.pdf
        private readonly IRestClient _client;
        private readonly IMapper _mapper;

        public Jobs2CareersProvider(IRestClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(string term, JobRequestSort sort, IEnumerable<JobFilter> jobType, Location location, int page, bool highlight,
            CancellationToken token)
        {
            if (location == null)
            {
                return null;
            }

            var nvc = new NameValueCollection
            {
                ["id"] = 4538.ToString(),
                ["pass"] = "PGhLlpYJEEeZjxCd",
                ["ip"] = location.Ip,
                ["l"] = $"{location.Address?.City}-{location.Address?.CountryCode}",
                ["start"] = (page * JobSearch.PageSize).ToString(),
                ["Limit"] = JobSearch.PageSize.ToString(),
                ["format"] = "json",
                ["link"] = 1.ToString()
            };

            if (sort == JobRequestSort.Date)
            {
                nvc.Add("sort", "d");
            }
            else
            {
                nvc.Add("d", JobSearch.RadiusOfFindingJobKm.ToString(CultureInfo.InvariantCulture));
            }
            var jobFilter = new List<string>();
            foreach (var filter in jobType ?? Enumerable.Empty<JobFilter>())
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

            var jobs = _mapper.Map<IEnumerable<JobDto>>(result);

            return new ResultWithFacetDto<JobDto>
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
            //public int total { get; set; }
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
            //public object[] zipcode { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            //public string price { get; set; }
            //public int preview { get; set; }
            //public string id { get; set; }
            //public string major_category0 { get; set; }
            //public string minor_category0 { get; set; }
        }


    }
}
