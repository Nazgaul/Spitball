using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Cloudents.Mvc4WebRole.Helpers;

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
        [NonAjax]
        [HttpGet]
        [CompressFilter]
        public async Task<ActionResult> Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var result = await PerformSearch(q, true);
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }
            JsonNetSerializer serializer = new JsonNetSerializer();

            ViewBag.data = serializer.Serialize(result);
            return View();
        }

        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Minute * 10)]
        public async Task<ActionResult> DropDown(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return new EmptyResult();
            }
            var result = await PerformSearch(q, false);

            return this.CdJson(new JsonResponse(true, result));

        }

        [Ajax, HttpGet, AjaxCache(TimeToCache = TimeConsts.Minute * 10)]
        public async Task<ActionResult> Data(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return new EmptyResult();
            }
            var result = await PerformSearch(q, true);

            return this.CdJson(new JsonResponse(true, result));
        }
        [NonAction]
        private async Task<Zbox.ViewModel.DTOs.Search.SearchDto> PerformSearch(string q, bool AllResult)
        {
            var userDetail = m_FormsAuthenticationService.GetUserData();
            var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, GetUserId(), AllResult);
            var result = await m_ZboxReadService.Search(query);
            return result;
        }


    }
}