using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
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
        [Route("Dashboard",Name ="dashboard")]
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

        [HttpGet, ActionName("BoxList")]
        public async Task<JsonResult> BoxListAsync()
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

        [HttpGet, ActionName("University")]
        //TODO: add output cache
        public async Task<JsonResult> UniversityAsync(long? universityId)
        {
            // ReSharper disable once PossibleInvalidOperationException - universityid have value because no university attribute
            var universityWrapper = universityId ?? User.GetUniversityId().Value;


            var query = new UniversityQuery(universityWrapper);
            var model = await ZboxReadService.GetUniversityInfoAsync(query);

            model.Url = Url.RouteUrlCache("universityLibrary", new RouteValueDictionary
            {
                ["universityId"] = model.Id,
                ["universityName"] = UrlConst.NameToQueryString(model.Name)
            });
            return JsonOk(model);

        }

       

        #region CreateBox

        [HttpPost, ActionName("Create")]
        // [ValidateAntiForgeryToken]
        public async Task<JsonResult> CreateAsync(CreateBox model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var userId = User.GetUserId();
                var command = new CreateBoxCommand(userId, model.BoxName);
                var result = await ZboxWriteService.CreateBoxAsync(command);
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
       
    }
}
