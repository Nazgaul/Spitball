using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Dashboard;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversity]
    public class DashboardController : BaseController
    {
        [AllowAnonymous]
        [DonutOutputCache(CacheProfile = "FullPage")]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("homePage");
            }
            return View("Empty");
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index");
        }

        [HttpGet]
        public async Task<JsonResult> BoxList()
        {
            var userid = User.GetUserId();
            try
            {
                var query = new GetBoxesQuery(userid);
                var data = await ZboxReadService.GetUserBoxesAsync(query);
                return JsonOk(data);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"BoxList user: {userid}", ex);
                return JsonError();
            }
        }

        [HttpGet]
        //TODO: add output cache
        public async Task<JsonResult> University()
        {
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = User.GetUniversityId().Value;

            var query = new UniversityQuery(universityWrapper);
            var model = await ZboxReadService.GetUniversityInfoAsync(query);
            return JsonOk(model);

        }

        [HttpGet]
        //TODO: add output cache
        public async Task<JsonResult> Leaderboard()
        {
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = User.GetUniversityId().Value;

            var query = new LeaderBoardQuery(universityWrapper);
            var model = await ZboxReadService.GetDashboardLeaderBoardAsync(query);
            return JsonOk(model);

        }

        #region CreateBox

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBox model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var userId = User.GetUserId();
                var command = new CreateBoxCommand(userId, model.BoxName);
                var result = ZboxWriteService.CreateBox(command);
                //return JsonOk(new { result.Url, result.Id });
                return JsonOk(new { result.Url });

            }
            catch (BoxNameAlreadyExistsException)
            {
                return JsonError(BoxControllerResources.BoxExists);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"CreateNewBox user: {User.GetUserId()} model: {model}", ex);
                return JsonError(BoxControllerResources.DashboardController_Create_Problem_with_Create_new_box);
            }
        }

        #endregion


        [HttpGet]
        public async Task<JsonResult> RecommendedCourses()
        {
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = User.GetUniversityId().Value;

            var query = new RecommendedCoursesQuery(universityWrapper, User.GetUserId());
            var result = await ZboxReadService.GetRecommendedCoursesAsync(query);
            return JsonOk(result.Select(s => new
            {
                s.CourseCode,
                s.ItemCount,
                s.MembersCount,
                s.Name,
                s.Professor,
                s.Url
            }));
        }

        

    }
}
