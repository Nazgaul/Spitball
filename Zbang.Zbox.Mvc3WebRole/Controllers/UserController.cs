using DevTrends.MvcDonutCaching;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Autocomplete;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class UserController : BaseController
    {
        public UserController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }

        [NoCache]
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Friends()
        {
            //var userid = GetUserId();
            //var users = m_ZboxReadService.GetUserFriends(new GetUserFriendsQuery(userid));
            return Json(new JsonResponse(true, null));
        }

        [HttpPost]
        [NoCache]
        //[DonutOutputCache(Duration = TimeConsts.Minute, Location = OutputCacheLocation.ServerAndClient)] // need to put cache for user as well
        public JsonResult FriendsByPrefix(string term)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new JsonResponse(false));
            }
            try
            {
                var query = new GetUserFriendsQueryByPrefix(GetUserId(), term);
                var result = m_ZboxReadService.GetUserFriendsByPrefix(query);
                return Json(new JsonResponse(true, result.Select(s => new { label = s.Name, value = s.Uid })));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("GetUserFriends user {0}", User.Identity.Name), ex);
                return Json(new JsonResponse(false, "Problem with get user friends"));
            }
        }

        [HttpGet]
        [ZboxAuthorize]
        [Ajax]
        [DonutOutputCache(Duration = TimeConsts.Hour, NoStore = true, Location = OutputCacheLocation.ServerAndClient, VaryByParam = "term")]
        public ActionResult University(string term)
        {
            var query = new GetUniversityQuery(term, GetUserId());
            var result = m_ZboxReadService.GetUniversityByPrefix(query);
            return Json(new JsonResponse(true, result), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ZboxAuthorize]
        [Ajax]
        [DonutOutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Client, NoStore = true, VaryByParam = "term")]
        public ActionResult Course(string term)
        {
            //if (!Request.IsAjaxRequest())
            //{
            //    return new EmptyResult();
            //}
            //var query = new GetAcademicBoxByCourseCodeQuery(term, GetUserId());
            //var result = m_ZboxReadService.GetBoxNameIdByCoursePrefix(query);
            //return Json(new JsonResponse(true, result.Select(s => new { label = s.Name, value = s.Uid })), JsonRequestBehavior.AllowGet);
            return Json(new JsonResponse(true, string.Empty), JsonRequestBehavior.AllowGet);
        }



    }
}
