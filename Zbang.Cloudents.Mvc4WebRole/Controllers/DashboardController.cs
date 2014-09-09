using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Library;
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
        [NoCache]
        public async Task<ActionResult> Index()
        {
            var userDetail = FormsAuthenticationService.GetUserData();
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = userDetail.UniversityId.Value;

            var query = new GetDashboardQuery(universityWrapper);
            var model = await ZboxReadService.GetMyData(query);

            if (Request.IsAjaxRequest())
            {
                return PartialView("Index2", model);
            }
            return View("Index2", model);
        }


        [Ajax]
        [HttpGet]
        public async Task<ActionResult> BoxList()
        {
            var userid = GetUserId();
            try
            {
                var query = new GetBoxesQuery(userid);
                var data = await ZboxReadService.GetDashboard(query);
                return Json(new JsonResponse(true, data));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("BoxList user: {0}", userid), ex);
                return Json(new JsonResponse(false));
            }
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
                var userId = GetUserId();
                var command = new CreateBoxCommand(userId, model.BoxName);
                var result = ZboxWriteService.CreateBox(command);
                var retVal = result.NewBox.Url;
                return Json(new JsonResponse(true, new { Url = retVal }));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateNewBox user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, BoxControllerResources.DashboardController_Create_Problem_with_Create_new_box);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }

        #endregion

        #region Friends
        [HttpGet, Ajax]
        [OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any, VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]
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

        [Ajax, HttpGet]
        public async Task<JsonResult> RecommendedCourses()
        {
            var query = new QueryBase(GetUserId());
            var result = await ZboxReadService.GetRecommendedCourses(query);
            return Json(new JsonResponse(true, result));
        }

        [Ajax, HttpGet]
        [Route("dashboard/CreateBox")]
        //[OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any, VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]
        public async Task<ActionResult> CreateBox()
        {
            try
            {
                var query = new QueryBase(GetUserId());
                var result = await ZboxReadService.GetDepartmentByUser(query);
                return PartialView("_CreateBoxWizard", result ?? new NodeDto());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PrivateBoxPartial ", ex);
                return Json(new JsonResponse(false));
            }
        }

    }
}
