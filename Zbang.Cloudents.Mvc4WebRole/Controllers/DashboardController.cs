using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.User;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversity]
    public class DashboardController : BaseController
    {
        //TODO: split between ajax and non ajax
        [NoCache]
        public async Task<ActionResult> Index()
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = userDetail.UniversityId.Value;

            var query = new GetDashboardQuery(universityWrapper);
            var model = await ZboxReadService.GetMyData(query);
            if (model == null) return RedirectToAction("Choose", "Library");

            if (Request.IsAjaxRequest())
            {
                return PartialView("Index2", model);
            }
            return View("Index2", model);
        }

        [HttpGet]
        [DonutOutputCache(Duration = TimeConsts.Minute * 5,
           Location = OutputCacheLocation.ServerAndClient,
           VaryByCustom = CustomCacheKeys.Lang, Options = OutputCacheOptions.IgnoreQueryString, VaryByParam = "none")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");
        }

        [HttpGet]
        public async Task<JsonResult> BoxList()
        {
            var userid = User.GetUserId();
            try
            {
                var query = new GetBoxesQuery(userid);
                var data = await ZboxReadService.GetUserBoxes(query);

                return JsonOk(data);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("BoxList user: {0}", userid), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpGet]
        public async Task<JsonResult> SideBar()
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = userDetail.UniversityId.Value;

            var query = new GetDashboardQuery(universityWrapper);
            var model = await ZboxReadService.GetDashboardSideBar(query);

            return JsonOk(model);

        }

        #region CreateBox

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBox model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var userId = User.GetUserId();
                var command = new CreateBoxCommand(userId, model.BoxName);
                var result = ZboxWriteService.CreateBox(command);
                return Json(new JsonResponse(true, new { result.Url, result.Id }));

            }
            catch (BoxNameAlreadyExistsException)
            {
                ModelState.AddModelError(string.Empty, BoxControllerResources.BoxExists);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateNewBox user: {0} model: {1}", User.GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, BoxControllerResources.DashboardController_Create_Problem_with_Create_new_box);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }

        #endregion

        #region Friends
        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
        public ActionResult FriendsPartial()
        {
            try
            {
                return PartialView("_Friends2");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("FriendsPartial", ex);
                return Json(new JsonResponse(false));
            }
        }
        #endregion

        [HttpGet]
        public async Task<JsonResult> RecommendedCourses()
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = userDetail.UniversityDataId.Value;

            var query = new RecommendedCoursesQuery(universityWrapper);
            var result = await ZboxReadService.GetRecommendedCourses(query);
            return Json(new JsonResponse(true, result));
        }

        [HttpGet]
        [Route("dashboard/CreateBox")]
        [OutputCache(CacheProfile = "PartialCache")]
        public ActionResult CreateBox()
        {
            try
            {
                return PartialView("_CreateBoxWizard");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PrivateBoxPartial ", ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
        public ActionResult SocialInvitePartial()
        {
            try
            {
                return PartialView("_Invite");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_Invite", ex);
                return Json(new JsonResponse(false));
            }
        }

    }
}
