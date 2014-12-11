using System;
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
        [AllowAnonymous]
        [RedirectToMobile]
        [DonutOutputCache(VaryByCustom = CustomCacheKeys.Lang,
           Duration = TimeConsts.Hour * 1,
           Location = OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            return View("Empty");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage",
           Options = OutputCacheOptions.IgnoreQueryString
           )]
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
                return JsonError();
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
                return JsonError(GetModelStateErrors());
            }
            try
            {
                var userId = User.GetUserId();
                var command = new CreateBoxCommand(userId, model.BoxName);
                var result = ZboxWriteService.CreateBox(command);
                return JsonOk(new { result.Url, result.Id });

            }
            catch (BoxNameAlreadyExistsException)
            {
                ModelState.AddModelError(string.Empty, BoxControllerResources.BoxExists);
                return JsonError(GetModelStateErrors());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateNewBox user: {0} model: {1}", User.GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, BoxControllerResources.DashboardController_Create_Problem_with_Create_new_box);
                return JsonError(GetModelStateErrors());
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
            return JsonOk(result);
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
                return JsonError();
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
                return JsonError();
            }
        }

    }
}
