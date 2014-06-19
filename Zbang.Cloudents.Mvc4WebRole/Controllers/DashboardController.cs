using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.User;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversity]
    public class DashboardController : BaseController
    {


        public DashboardController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService
            )
            : base(zboxWriteService, zboxReadService,
                formsAuthenticationService)
        {
        }

        [UserNavNWelcome]
        [AjaxCache(TimeConsts.Day)]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Index()
        {
            var userid = GetUserId();

            var userDetail = m_FormsAuthenticationService.GetUserData();

            if (userDetail.UniversityId == null)
            {
                return RedirectToAction("Choose", "Library");
            }
            var universityWrapper = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;

            var query = new GetDashboardQuery(userid, universityWrapper);
            var model = await m_ZboxReadService.GetMyData(query);

            if (Request.IsAjaxRequest())
            {
                return View("Index2", model);
            }
            return View("Index2", model);
        }





        [Ajax]
        [HttpGet]
        [AjaxCache(TimeConsts.Day)]
        public async Task<ActionResult> BoxList()
        {
            var userid = GetUserId();
            try
            {
                var query = new GetBoxesQuery(userid);
                var data = await m_ZboxReadService.GetDashboard(query);
                return this.CdJson(new JsonResponse(true, data));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("BoxList user: {0}", userid), ex);
                return this.CdJson(new JsonResponse(false));
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
                var command = new CreateBoxCommand(userId, model.BoxName, model.privacySettings);
                var retVal = CreateBox(command);
                return this.CdJson(new JsonResponse(true, new { Url = retVal }));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateNewBox user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, BoxControllerResources.DashboardController_Create_Problem_with_Create_new_box);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }


        [NonAction]
        private string CreateBox(CreateBoxCommand command)
        {
            var result = m_ZboxWriteService.CreateBox(command);
            return result.NewBox.Url;
        }

        //TODO: check this out
        [HttpGet, Ajax]
        public ActionResult PrivateBoxPartial()
        {
            try
            {
                return PartialView("_PrivateBoxDialog2");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("PrivateBoxPartial ", ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        #endregion

        #region search
        [HttpGet]
        [NonAjax]
        public ActionResult Search(string query)
        {
            return RedirectToActionPermanent("Index", "Search", new { q = query });
        }


        #endregion search


        //TODO: check this out
        #region Friends
        [HttpGet, Ajax]
        public ActionResult FriendsPartial()
        {
            try
            {
                return PartialView("_Friends2");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("FriendsPartial", ex);
                return this.CdJson(new JsonResponse(false));
            }
        }
        #endregion

    }
}
