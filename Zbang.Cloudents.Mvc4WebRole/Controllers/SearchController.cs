using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize]
    [NoUniversity]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class SearchController : BaseController
    {
        private readonly IBoxReadSearchProvider m_BoxSearchService;
        private readonly IItemReadSearchProvider m_ItemSearchService;

        public SearchController(IBoxReadSearchProvider boxSearchService, IItemReadSearchProvider itemSearchService)
        {
            m_BoxSearchService = boxSearchService;
            m_ItemSearchService = itemSearchService;
        }

        //[HttpGet]
        //[DonutOutputCache(CacheProfile = "PartialPage")]
        //public ActionResult IndexPartial()
        //{
        //    return PartialView("Index");
        //}



        [HttpGet]
        public async Task<ActionResult> Data(string q, int page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return JsonError("need query");
                }
                var result = await PerformSearch(q, page);

                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/Data q: " + q + " page: " + page + "userid: " + User.GetUserId(), ex);
                return JsonError("need query");
            }
        }
        [NonAction]
        private async Task<SearchDto> PerformSearch(string q, int page)
        {
            var universityDataId = User.GetUniversityData();
            if (!universityDataId.HasValue) return null;


            var t1 = m_BoxSearchService.SearchBox(new BoxSearchQuery(q, User.GetUserId(),
                universityDataId.Value, page
                ));
            var t2 =
                m_ItemSearchService.SearchItem(new ItemSearchQuery(q, User.GetUserId(),
                    universityDataId.Value, page));

            await Task.WhenAll(t1, t2);
            var retVal = new SearchDto
            {
                Boxes = t1.Result,
                Items = t2.Result
            };
            return retVal;
        }



        [HttpGet]
        public async Task<ActionResult> OtherUniversities(string q, int page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return JsonError("need query");
                }

                var universityId = User.GetUniversityId();

                if (!universityId.HasValue) return JsonError("need university");
                var query = new GroupSearchQuery(q, universityId.Value, User.GetUserId(), page, 50);
                var result = await ZboxReadService.OtherUniversities(query);
                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On OtherUniversities q: " + q + " page: " + page + "userid: " + User.GetUserId(), ex);
                return JsonError("need query");
            }
        }

    }
}