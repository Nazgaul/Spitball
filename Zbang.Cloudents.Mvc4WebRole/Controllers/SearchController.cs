using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
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
        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage",
           Options = OutputCacheOptions.IgnoreQueryString
           )]
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
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId != null)
            {
                var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, User.GetUserId(), allResult, page);
                var result = await ZboxReadService.Search(query);
                return result;
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
                var userDetail = FormsAuthenticationService.GetUserData();
                if (userDetail.UniversityId == null) return JsonError("need university");
                var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, User.GetUserId(), true, page);
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