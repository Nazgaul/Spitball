//using Cloudents.Core.DTOs;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Models;
//using JetBrains.Annotations;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core;


//namespace Cloudents.Infrastructure.Search.Tutor
//{
//    /// <summary>
//    /// <remarks>https://gist.github.com/barbuza/4b3666fa88cd326f18f2c464c8e4487c</remarks>
//    /// </summary>
//    [UsedImplicitly]
//    public class TutorMeSearch : ITutorProvider
//    {
//        private const string UrlEndpoint = "https://tutorme.com/api/v1/tutors/";
//        private readonly IRestClient _restClient;

//        public TutorMeSearch(IRestClient restClient)
//        {
//            _restClient = restClient;
//        }

//        public Task<IEnumerable<TutorDto>> SearchAsync(string term,
//            TutorRequestFilter[] filters,
//            TutorRequestSort sort, GeoPoint location, int page, bool isMobile, CancellationToken token)
//        {
//            if (filters.Length > 0
//                && Array.TrueForAll(filters, t => t == TutorRequestFilter.InPerson))
//            {
//                return Task.FromResult<IEnumerable<TutorDto>>(null);
//            }

//            if (isMobile)
//            {
//                return Task.FromResult<IEnumerable<TutorDto>>(null);
//            }
//            return TutorMeApiAsync(term, page, token);
//        }

//        private async Task<IEnumerable<TutorDto>> TutorMeApiAsync(string term, int page, CancellationToken token)
//        {
//            // page is 12
//            var nvc = BuildQueryString(term, page);
//            var result = await _restClient.GetAsync<TutorMeResult>(
//                new Uri(UrlEndpoint), nvc, token);
//            if (result?.Group == null)
//            {
//                return null;
//            }

//            return result.Results.Select((source, i) => new TutorDto()
//            {
//                Description = source.About,
//                Fee = 60,
//                Image = source.Avatar.X300,
//                Name = source.ShortName,
//                Online = source.IsOnline,
//                //Source = "TutorMe",
//                PrioritySource = PrioritySource.TutorMe,
//                Url = $"https://tutorme.com/tutors/{source.Id}/",
//                Order = i + 1
//            });
//        }

//        public const int TutorMePage = 12;

//        private static NameValueCollection BuildQueryString(string term, int page)
//        {
//            return new NameValueCollection
//            {
//                ["search"] = term,
//                ["offset"] = (page * TutorMePage).ToString()
//            };
//        }

//        public class TutorMeResult
//        {
//            [JsonProperty("results")]
//            public Result[] Results { get; set; }

//            [JsonProperty("group")]
//            public Group Group { get; set; }

//            //public int count { get; set; }
//            //public object subject { get; set; }

//        }

//        public class Group
//        {
//            public string Name { get; set; }
//            public int Id { get; set; }
//        }

//        public class Result
//        {
//            [JsonProperty("about")]
//            public string About { get; set; }

//            [JsonProperty("shortName")]
//            public string ShortName { get; set; }

//            [JsonProperty("avatar")]
//            public Avatar Avatar { get; set; }

//            [JsonProperty("id")]
//            public int Id { get; set; }

//            [JsonProperty("isOnline")]
//            public bool IsOnline { get; set; }

//            //public string firstName { get; set; }
//            //public int gender { get; set; }
//            //public bool inSession { get; set; }
//            //public string tagline { get; set; }

//        }

//        public class Avatar
//        {
//            [JsonProperty("x300")]
//            public string X300 { get; set; }

//            //public string x80 { get; set; }

//        }
//    }
//}
