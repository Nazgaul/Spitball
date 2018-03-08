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
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Tutor
{
    /// <summary>
    /// <remarks>https://gist.github.com/barbuza/4b3666fa88cd326f18f2c464c8e4487c</remarks>
    /// </summary>
    [UsedImplicitly]
    public class TutorMeSearch : ITutorProvider
    {
        private const string UrlEndpoint = "https://tutorme.com/api/v1/tutors/";
        private readonly IMapper _mapper;
        private readonly IRestClient _restClient;

        public TutorMeSearch(IMapper mapper, IRestClient restClient)
        {
            _mapper = mapper;
            _restClient = restClient;
        }

        public Task<IEnumerable<TutorDto>> SearchAsync(string term,
            TutorRequestFilter[] filters,
            TutorRequestSort sort, GeoPoint location, int page, bool isMobile, CancellationToken token)
        {
            if (filters.Length > 0 && Array.TrueForAll(filters, t => t == TutorRequestFilter.InPerson))
            {
                return Task.FromResult<IEnumerable<TutorDto>>(null);
            }

            if (isMobile)
            {
                return Task.FromResult<IEnumerable<TutorDto>>(null);
            }
            return TutorMeApiAsync(term, page, token);
        }

        private async Task<IEnumerable<TutorDto>> TutorMeApiAsync(string term, int page, CancellationToken token)
        {
            // page is 12
            var nvc = BuildQueryString(term, page);
            var result = await _restClient.GetAsync<TutorMeResult>(
                new Uri(UrlEndpoint), nvc, token).ConfigureAwait(false);
            if (result?.Group == null)
            {
                return null;
            }
            return _mapper.Map<IEnumerable<TutorDto>>(result.Results);
        }

        private static NameValueCollection BuildQueryString(string term, int page)
        {
            var nvc = new NameValueCollection
            {
                ["search"] = term,
                ["offset"] = (page * 12).ToString()
            };
            return nvc;
        }

        public class TutorMeResult
        {
            //public int count { get; set; }
            [JsonProperty("results")]
            public Result[] Results { get; set; }
            //public object subject { get; set; }
            [JsonProperty("group")]
            public Group Group { get; set; }
        }

        public class Group
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class Result
        {
            //public int gender { get; set; }
            [JsonProperty("about")]
            public string About { get; set; }
            //public string tagline { get; set; }
            [JsonProperty("shortName")]
            public string ShortName { get; set; }
            //public bool inSession { get; set; }
            [JsonProperty("avatar")]
            public Avatar Avatar { get; set; }
            //public string firstName { get; set; }
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("isOnline")]
            public bool IsOnline { get; set; }
        }

        public class Avatar
        {
            //public string x80 { get; set; }
            [JsonProperty("x300")]
            public string X300 { get; set; }
        }


    }
}
