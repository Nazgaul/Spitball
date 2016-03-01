using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class SearchController : ApiController
    {
        private readonly IBoxReadSearchProvider2 m_BoxSearchService2;
        private readonly IItemReadSearchProvider2 m_ItemSearchService2;
        private readonly IUniversityReadSearchProvider m_UniversitySearch;
        private readonly IZboxCacheReadService m_ZboxReadService;

        public SearchController(IBoxReadSearchProvider2 boxSearchService2, IItemReadSearchProvider2 itemSearchService2, IUniversityReadSearchProvider universitySearch, IZboxCacheReadService zboxReadService)
        {
            m_BoxSearchService2 = boxSearchService2;
            m_ItemSearchService2 = itemSearchService2;
            m_UniversitySearch = universitySearch;
            m_ZboxReadService = zboxReadService;
        }


        // GET api/Search


        [HttpGet]
        //, VersionedRoute("api/search/boxes", 3)]
        [Route("api/search/boxes")]
        public async Task<HttpResponseMessage> Boxes(string term, int page, int sizePerPage = 20)
        {
            long? universityId = User.GetUniversityDataId();
            //var userDetail = FormsAuthenticationService.GetUserData();


            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");


            var query = new SearchQueryMobile(term, User.GetCloudentsUserId(), universityId.Value, page, sizePerPage);
            // Services.Log.Info(String.Format("search boxes query: {0}", query));
            var retVal = await m_BoxSearchService2.SearchBoxAsync(query, default(CancellationToken)) ?? new List<SearchBoxes>();

            return Request.CreateResponse(retVal.Where(w => w.Type != BoxType.AcademicClosed).Select(s => new
            {
                s.Id,
                s.Name,
                s.Professor,
                s.CourseCode,
                shortUrl = UrlConsts.BuildShortItemUrl(new Base62(s.Id).ToString()),
                BoxType = s.Type
            }));
        }


        
        [HttpGet]
        //[VersionedRoute("api/search/items", 2)]
        [Route("api/search/items")]
        public async Task<HttpResponseMessage> Items2(string term, int page, int sizePerPage = 20)
        {
            long? universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");

            var query = new SearchQueryMobile(term, User.GetCloudentsUserId(), universityId.Value, page, sizePerPage);
            // Services.Log.Info(String.Format("search items query: {0}", query));
            var retVal = await m_ItemSearchService2.SearchItemAsync(query, default(CancellationToken)) ?? new List<SearchItems>();
            return Request.CreateResponse(retVal.Select(s => new
            {
                s.Name,
                s.BoxId,
                s.Id,
                s.Extension,
                s.Content
            }));
        }
        [HttpGet]
        [Route("api/search/university")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> University(string term, int page, int sizePerPage = 20)
        {
            if (string.IsNullOrEmpty(term))
            {
                var ip = Request.GetClientIp();

                var retValWithoutSearch =
                    await
                        m_ZboxReadService.GetUniversityByIpAddressAsync(new UniversityByIpQuery(Ip2Long(ip), sizePerPage, page));

                retValWithoutSearch = retValWithoutSearch.Select(s =>
                {
                    if (string.IsNullOrEmpty(s.Image))
                    {
                        s.Image = "https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png";
                    }
                    return s;
                });

                return Request.CreateResponse(retValWithoutSearch);
            }
            var query = new UniversitySearchQuery(term, sizePerPage, page);
            var retVal = await m_UniversitySearch.SearchUniversityAsync(query, default(CancellationToken));

            retVal = retVal.Select(s =>
            {
                if (string.IsNullOrEmpty(s.Image))
                {
                    s.Image = "https://az32006.vo.msecnd.net/zboxprofilepic/S100X100/universityEmptyState.png";
                }
                return s;
            });
            return Request.CreateResponse(retVal);
        }

        [HttpGet, Route("api/search/user")]
        public async Task<HttpResponseMessage> Members(string term, long boxId, int page, int sizePerPage = 20)
        {
            if (string.IsNullOrEmpty(term))
            {
                term = "";
            }
            long? universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");
            var query = new UserSearchQuery(term, universityId.Value, boxId, page, sizePerPage);
            var retVal = await m_ZboxReadService.GetUsersByTermAsync(query);

            return Request.CreateResponse(retVal);

        }

        private static long Ip2Long(string ip)
        {
            double num = 0;
            if (string.IsNullOrEmpty(ip)) return (long)num;
            var ipBytes = ip.Split('.');
            for (var i = ipBytes.Length - 1; i >= 0; i--)
            {
                num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
            }
            return (long)num;
        }
    }
}
