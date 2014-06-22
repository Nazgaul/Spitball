using System;
using System.Linq;
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
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
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
            return View("Empty", model);
        }

        private Zbox.ViewModel.DTOs.Dashboard.DashboardDto AssignUrl(Zbox.ViewModel.DTOs.Dashboard.DashboardDto data)
        {
            var builder = new UrlBuilder(HttpContext);
            data.Boxes = data.Boxes.Select(s =>
            {
                s.Url = builder.BuildBoxUrl(s.BoxType, s.Id, s.Name, s.UniName);
                return s;
            });
            data.Wall = data.Wall.Select(item =>
            {
                item.Url = builder.BuildBoxUrl(item.BoxId, item.BoxName, item.UniName);
                return item;
            });
            return data;

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
                data = AssignUrl(data);
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
                return this.CdJson(new JsonResponse(true, retVal));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateNewBox user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, BoxControllerResources.DashboardController_Create_Problem_with_Create_new_box);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }


        [NonAction]
        private BoxDto CreateBox(CreateBoxCommand command)
        {
            var result = m_ZboxWriteService.CreateBox(command);
            var builder = new UrlBuilder(HttpContext);
            //TODO: User name can come from cookie detail one well do that
            var retVal = new BoxDto(result.NewBox.Id, command.BoxName,
                 UserRelationshipType.Owner, 0,
                 0, 0, string.Empty, string.Empty, BoxType.Box, string.Empty,
                 builder.BuildBoxUrl(BoxType.Box, result.NewBox.Id, command.BoxName, string.Empty)
                 );
            return retVal;
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
        [HttpGet,  Ajax]
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
