using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [ZboxAuthorize]
    [NoUniversity]
    public class SearchController : BaseController
    {
        public SearchController(IZboxReadService zboxReadService, IZboxWriteService zboxWriteService, IFormsAuthenticationService formAuthService)
            : base(zboxWriteService, zboxReadService, formAuthService)
        {
        }
        //
        // GET: /Search/
        [UserNavNWelcome]
        [HttpGet]
        public async Task<ActionResult> Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var result = await PerformSearch(q, true, 0);
            var serializer = new JsonNetSerializer();
            ViewBag.data = serializer.Serialize(result);
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return View();
        }

        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Minute * 10)]
        public async Task<ActionResult> DropDown(string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return this.CdJson(new JsonResponse(false, "need query"));
                }
                var result = await PerformSearch(q, false, 0);

                return this.CdJson(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/DropDown q: " + q + "userid: " + GetUserId(), ex);
                return this.CdJson(new JsonResponse(false, "need query"));
            }
        }

        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Minute * 10)]
        public async Task<ActionResult> Data(string q, int page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return this.CdJson(new JsonResponse(false, "need query"));
                }
                var result = await PerformSearch(q, true, page);

                return this.CdJson(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Seach/Data q: " + q + " page: " + page + "userid: " + GetUserId(), ex);
                return this.CdJson(new JsonResponse(false, "need query"));
            }
        }
        [NonAction]
        private async Task<Zbox.ViewModel.DTOs.Search.SearchDto> PerformSearch(string q, bool allResult, int page)
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId != null)
            {
                var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, GetUserId(), allResult, page);
                var result = await ZboxReadService.Search(query);
                return result;
            }
            return null;
        }
       


        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Minute * 10)]
        public async Task<ActionResult> OtherUniversities(string q, int page)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return this.CdJson(new JsonResponse(false, "need query"));
                }
                var userDetail = FormsAuthenticationService.GetUserData();
                if (userDetail.UniversityId != null)
                {
                    var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, GetUserId(), true, page);
                    var result = await ZboxReadService.OtherUniversities(query);
                    return this.CdJson(new JsonResponse(true, result));
                }
                return this.CdJson(new JsonResponse(false, "need univeristy"));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On OtherUniversities q: " + q + " page: " + page + "userid: " + GetUserId(), ex);
                return this.CdJson(new JsonResponse(false, "need query"));
            }
        }

    }
}