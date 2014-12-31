using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Controllers.Resources;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.User;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversity]
    public class DashboardController : BaseController
    {



        [HttpGet]
        [AllowAnonymous]
        [DonutOutputCache(CacheProfile = "PartialPage",
           Options = OutputCacheOptions.IgnoreQueryString
           )]
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        [HttpGet]
        public async Task<JsonResult> BoxList(int page)
        {
            try
            {
                var query = new GetBoxesQuery(User.GetUserId(), page, 15);
                var data = await ZboxReadService.GetUserBoxes(query);

                return JsonOk(data.Select(s => new { s.Name, s.BoxPicture, s.Url, s.ItemCount, s.CommentCount }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("BoxList user: {0}", User.GetUserId()), ex);
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

        //[HttpGet]
        //[Route("dashboard/CreateBox")]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult CreateBox()
        //{
        //    try
        //    {
        //        return PartialView("_CreateBoxWizard");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("PrivateBoxPartial ", ex);
        //        return JsonError();
        //    }
        //}

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult SocialInvitePartial()
        //{
        //    try
        //    {
        //        return PartialView("_Invite");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_Invite", ex);
        //        return JsonError();
        //    }
        //}

    }
}
