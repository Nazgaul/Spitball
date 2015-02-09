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

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        [HttpGet]
        public async Task<ActionResult> DropDown(string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return JsonError("need query");
                }
                var result = await PerformSearch(q, false, 0);

                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/DropDown q: " + q + "userid: " + User.GetUserId(), ex);
                return JsonError("need query");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Data(string q, int page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return JsonError("need query");
                }
                var result = await PerformSearch(q, true, page);

                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/Data q: " + q + " page: " + page + "userid: " + User.GetUserId(), ex);
                return JsonError("need query");
            }
        }
        [NonAction]
        private async Task<SearchDto> PerformSearch(string q, bool allResult, int page)
        {
            var universityDataId = User.GetUniversityData();
            if (universityDataId.HasValue)
            {
                var query = new GroupSearchQuery(q, universityDataId.Value, User.GetUserId(), page,
                    allResult ? 50 : 6);

                var t1 = m_BoxSearchService.SearchBox(new BoxSearchQuery(q, User.GetUserId(),
                    universityDataId.Value, page,
                     allResult ? 50 : 6));
                var t2 =
                    m_ItemSearchService.SearchItem(new ItemSearchQuery(q, User.GetUserId(),
                    universityDataId.Value, page,
                        allResult ? 50 : 6));
                var t3 = ZboxReadService.Search(query);
                await Task.WhenAll(t1, t2, t3);
                t3.Result.Boxes = t1.Result;
                t3.Result.Items = t2.Result;
                return t3.Result;
            }
            return null;
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