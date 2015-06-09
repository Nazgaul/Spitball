using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.Extensions;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class SearchController : ApiController
    {
        public IBoxReadSearchProvider BoxSearchService { get; set; }
        public IItemReadSearchProvider ItemSearchService { get; set; }

        public IUniversityReadSearchProvider UniversitySearch { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }
        public ApiServices Services { get; set; }

        // GET api/Search
        [HttpGet]
        [Route("api/search/boxes")]
        public async Task<HttpResponseMessage> Boxes(string term, int page, int sizePerPage = 20)
        {
            long? universityId = User.GetUniversityDataId();
            //var userDetail = FormsAuthenticationService.GetUserData();


            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");

            
            var query = new SearchQuery(term, User.GetCloudentsUserId(), universityId.Value, page, sizePerPage);
           // Services.Log.Info(String.Format("search boxes query: {0}", query));
            var retVal = await BoxSearchService.SearchBox(query, default(CancellationToken)) ?? new List<SearchBoxes>();

            return Request.CreateResponse(retVal.Select(s => new
             {
                 s.Id,
                 s.Name,
                 s.Professor,                 
                 s.CourseCode,
                 s.Url,
                 shortUrl = UrlConsts.BuildShortItemUrl(new Base62(s.Id).ToString())
             }));
        }

        [HttpGet]
        [Route("api/search/items")]
        public async Task<HttpResponseMessage> Items(string term, int page,  int sizePerPage = 20)
        {
            long? universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");

            var query = new SearchQuery(term, User.GetCloudentsUserId(), universityId.Value, page, sizePerPage);
           // Services.Log.Info(String.Format("search items query: {0}", query));
            var retVal = await ItemSearchService.SearchItem(query, default(CancellationToken)) ?? new List<SearchItems>();
            return Request.CreateResponse(retVal.Select(s => new
            {
                s.Name,
                s.Id,
                s.Url
                
            }));
        }

        [HttpGet]
        [Route("api/search/university")]
        [AuthorizeLevel(AuthorizationLevel.Application)]
        public async Task<HttpResponseMessage> University(string term, int page, int sizePerPage = 20)
        {
            if (string.IsNullOrEmpty(term))
            {
                 var ip = Request.GetClientIp();
                
                var retValWithoutSearch =
                    await
                        ZboxReadService.GetUniversityByIpAddress(new UniversityByIpQuery(Ip2Long(ip), sizePerPage, page));
                return Request.CreateResponse(retValWithoutSearch);
            }
            var query = new UniversitySearchQuery(term, sizePerPage, page);
           // Services.Log.Info(String.Format("search university query: {0}", query));
            var retVal = await UniversitySearch.SearchUniversity(query);

            return Request.CreateResponse(retVal);
        }

        private static long Ip2Long(string ip)
        {
            double num = 0;
            if (string.IsNullOrEmpty(ip)) return (long) num;
            var ipBytes = ip.Split('.');
            for (var i = ipBytes.Length - 1; i >= 0; i--)
            {
                num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
            }
            return (long)num;
        }
    }
}
