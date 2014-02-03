using DevTrends.MvcDonutCaching;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Factories;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : BaseController
    {
        public HomeController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }

        [ZboxAuthorize]
        [OutputCache(Duration = TimeConsts.Day, Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Index()
        {
            return View();
        }

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = "Lang")]
        public ActionResult ContactUs()
        {
            return View();
        }

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = "Lang")]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = "Lang")]
        public ActionResult Privacy()
        {
            return View();
        }

        [DonutOutputCache(Duration = TimeConsts.Day, VaryByParam = "None", VaryByCustom = "Lang")]
        public ActionResult AboutUs()
        {
            return View();
        }

        [ValidateInput(false)]
        [ZboxAuthorize]
        public ActionResult SubmitSearch(string query)
        {
            return RedirectToAction("Search", new { query = HttpUtility.HtmlEncode(query) });
        }


        [ZboxAuthorize]
        public ActionResult Search(string query)
        {
            ViewBag.query = HttpUtility.HtmlDecode(query);
            return View();
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult DoSearch(string query, int pageNumber = 0, SearchType type = SearchType.Box)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new JsonResponse(false));
            }
            var userId = GetUserId();

            try
            {

                var searchFactory = new SearchQueryFactory();

                var searchQuery = searchFactory.GetQuery(query, pageNumber, type, userId);
                var result = m_ZboxReadService.Search(searchQuery);
                return Json(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Do search query: {0}, pageNumber {1}, type {2},userid {3}", query, pageNumber, type, userId), ex);
                return new HttpStatusCodeResult(500);
            }
        }

        //public ActionResult test()
        //{
        //    //jQuery("body").append("$(Contents.Replace('%22','\%22').Replace('%0A','').Replace('%0D',''))%22)%3B
        //    var template = System.IO.File.ReadAllText(Server.MapPath("/Content/Templates/Boxes.html"));
        //    var script = "jQuery('body').append('" + template.Replace("%22", "\\%22").Replace("%0A", "").Replace("%0D", "") + "')";
        //    return JavaScript(script);
        //}


    }
}
