using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Search;
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
        private readonly IItemReadSearchProvider m_ItemSearchService2;
        private readonly IUniversityReadSearchProvider m_UniversitySearch;
        private readonly IZboxCacheReadService m_ZboxReadService;
       
        public SearchController(IBoxReadSearchProvider2 boxSearchService2, IItemReadSearchProvider itemSearchService2, IUniversityReadSearchProvider universitySearch, IZboxCacheReadService zboxReadService)
        {
            m_BoxSearchService2 = boxSearchService2;
            m_ItemSearchService2 = itemSearchService2;
            m_UniversitySearch = universitySearch;
            m_ZboxReadService = zboxReadService;
        }

        // GET api/Search

        [HttpGet]
        [Route("api/search/boxes")]
        public async Task<HttpResponseMessage> BoxesAsync(string term, int page, int sizePerPage = 20)
        {
            var cancelToken = Request.GetCancellationToken();
            var universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");

            var query = new SearchQueryMobile(term, User.GetUserId(), universityId.Value, page, sizePerPage);
            var retVal = await m_BoxSearchService2.SearchBoxAsync(query, cancelToken) ?? new List<SearchBoxes>();

            return Request.CreateResponse(retVal.Select(s => new
            {
                s.Id,
                s.Name,
                s.Professor,
                s.CourseCode,
               // shortUrl = UrlConst.BuildShortItemUrl(new Base62(s.Id).ToString()),
                s.Type,
                s.ItemCount,
                s.MembersCount,
                s.DepartmentId
            }));
        }

        [HttpGet]
        [Route("api/search/items")]
        public async Task<HttpResponseMessage> Items2Async(string term, int page, int sizePerPage = 20)
        {
            long? universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");
            var cancelToken = Request.GetCancellationToken();
            var query = new SearchQueryMobile(term, User.GetUserId(), universityId.Value, page, sizePerPage);
            // Services.Log.Info(String.Format("search items query: {0}", query));
            var retVal = await m_ItemSearchService2.SearchItemAsync(query, cancelToken) ?? new List<SearchDocument>();
            return Request.CreateResponse(retVal.Select(s => new
            {
                s.Name,
                s.BoxId,
                s.Id,
                s.Extension,
                s.Content,
                s.Source
            }));
        }

        [HttpGet]
        [Route("api/search/university")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> UniversityAsync(string term, int page, int sizePerPage = 20)
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
            var cancelToken = Request.GetCancellationToken();

            var query = new UniversitySearchQuery(term, sizePerPage, page);
            var retVal = await m_UniversitySearch.SearchUniversityAsync(query, cancelToken);

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
        public async Task<HttpResponseMessage> MembersAsync(string term, long boxId, int page, int sizePerPage = 20)
        {
            if (string.IsNullOrEmpty(term))
            {
                term = "";
            }
            long? universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");
            var query = new UserInBoxSearchQuery(term, universityId.Value, boxId, page, sizePerPage);
            var retVal = await m_ZboxReadService.GetUsersInBoxByTermAsync(query);

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
