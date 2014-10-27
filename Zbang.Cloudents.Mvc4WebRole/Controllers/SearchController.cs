using System;
using System.Threading.Tasks;
using System.Web.Mvc;
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
        //[UserNavNWelcome]
        [HttpGet]
        public /*async Task<ActionResult>*/ ActionResult Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index", "Dashboard");
            }
       
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View("Empty");
        }

        [Ajax, HttpGet]
        public async Task<ActionResult> DropDown(string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return Json(new JsonResponse(false, "need query"));
                }
                var result = await PerformSearch(q, false, 0);

                return Json(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/DropDown q: " + q + "userid: " + User.GetUserId(), ex);
                return Json(new JsonResponse(false, "need query"));
            }
        }

        [Ajax, HttpGet]
        public async Task<ActionResult> Data(string q, int page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return Json(new JsonResponse(false, "need query"));
                }
                var result = await PerformSearch(q, true, page);

                return Json(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/Data q: " + q + " page: " + page + "userid: " + User.GetUserId(), ex);
                return Json(new JsonResponse(false, "need query"));
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
       


        [Ajax, HttpGet]
        public async Task<ActionResult> OtherUniversities(string q, int page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return Json(new JsonResponse(false, "need query"));
                }
                var userDetail = FormsAuthenticationService.GetUserData();
                if (userDetail.UniversityId != null)
                {
                    var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, User.GetUserId(), true, page);
                    var result = await ZboxReadService.OtherUniversities(query);
                    return Json(new JsonResponse(true, result));
                }
                return Json(new JsonResponse(false, "need univeristy"));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On OtherUniversities q: " + q + " page: " + page + "userid: " + User.GetUserId(), ex);
                return Json(new JsonResponse(false, "need query"));
            }
        }

    }
}