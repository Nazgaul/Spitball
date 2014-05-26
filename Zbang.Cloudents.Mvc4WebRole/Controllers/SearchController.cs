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
        [CompressFilter]
        public async Task<ActionResult> Index(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var result = await PerformSearch(q, true, 0);
            JsonNetSerializer serializer = new JsonNetSerializer();
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
            var userDetail = m_FormsAuthenticationService.GetUserData();
            if (userDetail.UniversityId != null)
            {
                var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, GetUserId(), allResult, page);
                var result = await m_ZboxReadService.Search(query);
                var urlBuilder = new UrlBuilder(HttpContext);
                AssignUrls(result, urlBuilder);

                return result;
            }
            return null;
        }
        [NonAction]
        private void AssignUrls(Zbox.ViewModel.DTOs.Search.SearchDto result, UrlBuilder urlBuilder)
        {
            if (result.Boxes != null)
            {
                result.Boxes = result.Boxes.Select(s =>
                {
                    s.Url = urlBuilder.BuildBoxUrl(s.Id, s.Name, s.Universityname);
                    return s;
                });
            }
            if (result.Items != null)
            {
                result.Items = result.Items.Select(s =>
                {
                    s.Url = urlBuilder.buildItemUrl(s.Boxid, s.Boxname, s.Id, s.Name, s.Universityname);
                    return s;
                });
            }
            if (result.OtherItems != null)
            {
                result.OtherItems = result.OtherItems.Select(s =>
                {
                    s.Url = urlBuilder.buildItemUrl(s.Boxid, s.Boxname, s.Id, s.Name, s.Universityname);
                    return s;
                });
            }
            if (result.Users != null)
            {
                result.Users = result.Users.Select(s =>
                {
                    s.Url = urlBuilder.BuildUserUrl(s.Id, s.Name);
                    return s;
                });
            }
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
                var userDetail = m_FormsAuthenticationService.GetUserData();
                if (userDetail.UniversityId != null)
                {
                    var query = new GroupSearchQuery(q, userDetail.UniversityId.Value, GetUserId(), true, page);
                    var result = await m_ZboxReadService.OtherUniversities(query);
                    var urlBuilder = new UrlBuilder(HttpContext);


                    result = result.Select(s =>
                    {
                        s.Url = urlBuilder.buildItemUrl(s.Boxid, s.Boxname, s.Id, s.Name, s.Universityname);
                        return s;
                    });
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