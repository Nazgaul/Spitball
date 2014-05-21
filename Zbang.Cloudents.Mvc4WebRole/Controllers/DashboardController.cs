using DevTrends.MvcDonutCaching;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        { }

        [UserNavNWelcome]
        [AjaxCache(TimeConsts.Day)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [CompressFilter(Order = 1)]
        // [ETag(Order = 2)]
        //[FlushHeader]
        public async Task<ActionResult> Index()
        {
            var userid = GetUserId();

            var userDetail = m_FormsAuthenticationService.GetUserData();

            var universityWrapper = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;

            var query = new GetDashboardQuery(userid, universityWrapper);
            var taskData = await m_ZboxReadService.GetMyData(query);

            //var queryboxes = new GetBoxesQuery(userid);
            //var taskBoxes = m_ZboxReadService.GetDashboard(queryboxes);

            //await Task.WhenAll(taskData, taskBoxes);
            //var data = taskBoxes.Result;
            //data = AssignUrl(data);
            //JsonNetSerializer serializer = new JsonNetSerializer();
            //ViewBag.Boxes = serializer.Serialize(data);

            if (Request.IsAjaxRequest())
            {
                return View("Index2", taskData);
            }
            return View("Index2", taskData);
        }

        private Zbang.Zbox.ViewModel.DTOs.Dashboard.DashboardDto AssignUrl(Zbang.Zbox.ViewModel.DTOs.Dashboard.DashboardDto data)
        {
            UrlBuilder builder = new UrlBuilder(this.HttpContext);
            data.Boxes = data.Boxes.Select(s =>
             {
                 s.Url = builder.BuildBoxUrl(s.BoxType, s.Id, s.Name, s.UniName);
                 return s;
             });
            foreach (var item in data.Wall)
            {
                item.Url = builder.BuildBoxUrl(item.BoxId, item.BoxName, item.UniName);
                item.UserUrl = builder.BuildUserUrl(item.UserId, item.UserName);
            }
            data.Friends = data.Friends.Select(s =>
             {
                 s.Url = builder.BuildUserUrl(s.Uid, s.Name);
                 return s;
             });
            return data;

        }



        [Ajax]
        [HttpGet]
        [CompressFilter(Order = 1)]
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
        [ValidateAntiForgeryToken]
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
                ModelState.AddModelError(string.Empty, "Problem with Create new box");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }


        [NonAction]
        private BoxDto CreateBox(CreateBoxCommand command)
        {
            var result = m_ZboxWriteService.CreateBox(command);
            UrlBuilder builder = new UrlBuilder(HttpContext);
            //TODO: User name can come from cookie detail one well do that
            var retVal = new BoxDto(result.NewBox.Id, command.BoxName,
                 UserRelationshipType.Owner, 0,
                 0, 0, string.Empty, string.Empty, BoxType.Box, string.Empty,
                 builder.BuildBoxUrl(BoxType.Box, result.NewBox.Id, command.BoxName, string.Empty)
                 );
            return retVal;
        }

        [HttpGet, CompressFilter, Ajax]
        public ActionResult PrivateBoxPartial()
        {
            try
            {
                return PartialView("_PrivateBoxDialog");
            }
            catch (Exception ex)
            {
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

        #region Friends
        [HttpGet, CompressFilter, Ajax]
        public ActionResult FriendsPartial()
        {
            try
            {
                return PartialView("_Friends2");
            }
            catch (Exception ex)
            {
                return this.CdJson(new JsonResponse(false));
            }
        }
        #endregion


    }
}
