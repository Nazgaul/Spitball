using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    public class ZipRecruiterClient : IJobSearch
    {
        private readonly IGooglePlacesSearch _googlePlacesSearch;
        private readonly IRestClient _client;

        public ZipRecruiterClient(IGooglePlacesSearch googlePlacesSearch, IRestClient client)
        {
            _googlePlacesSearch = googlePlacesSearch;
            _client = client;
        }

        public async Task<ResultWithFacetDto<JobDto>> SearchAsync(IEnumerable<string> term, JobRequestFilter filter, JobRequestSort sort, IEnumerable<string> jobType, GeoPoint location,
            int page, CancellationToken token)
        {
            var address = await _googlePlacesSearch.ReverseGeocodingAsync(location, token);
            var str = string.Join(" ", term ?? Enumerable.Empty<string>());
            var nvc = new NameValueCollection
            {
                ["location"] = address.Address,
                ["api_key"] = "x8w8rgmv2dq78dw5wfmwiwexwu3hdfv3",
                ["search"] = str,
                ["jobs_per_page"] = JobSearch.PageSize.ToString(),
                ["page"] = page.ToString()
            };
            var result = await _client.GetJsonAsync(new Uri("https://api.ziprecruiter.com/jobs/v1"), nvc, token);
            return null;
        }
    }

    public class ZipRecruiterDto
    {
        public int salary_max_annual { get; set; }
        public string snippet { get; set; }
        public string industry_name { get; set; }
        public string source { get; set; }
        public string state { get; set; }
        public Hiring_Company hiring_company { get; set; }
        public DateTime posted_time { get; set; }
        public string category { get; set; }
        public int salary_min { get; set; }
        public string salary_source { get; set; }
        public int salary_max { get; set; }
        public string country { get; set; }
        public int salary_min_annual { get; set; }
        public int job_age { get; set; }
        public string location { get; set; }
        public string salary_interval { get; set; }
        public string posted_time_friendly { get; set; }
        public string name { get; set; }
        public string has_non_zr_url { get; set; }
        public string url { get; set; }
        public string city { get; set; }
        public string id { get; set; }
    }

    public class Hiring_Company
    {
        public object id { get; set; }
        public object description { get; set; }
        public string name { get; set; }
        public object url { get; set; }
    }

}
