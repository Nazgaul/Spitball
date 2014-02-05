using System.Collections.Generic;
using DevTrends.MvcDonutCaching;
using System;
using System.Linq;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.User;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using System.Web.WebPages;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversityAttribute]
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
            var taskData = m_ZboxReadService.GetMyData(query);

            var queryboxes = new GetBoxesQuery(userid);
            var taskBoxes = m_ZboxReadService.GetDashboard(queryboxes);

            await Task.WhenAll(taskData, taskBoxes);
            var data = taskBoxes.Result;
            data = AssignUrl(data);
            JsonNetSerializer serializer = new JsonNetSerializer();

            ViewBag.Boxes = serializer.Serialize(data);


            if (Request.IsAjaxRequest())
            {
                return PartialView(taskData.Result);
            }
            return View(taskData.Result);
        }

        private Zbang.Zbox.ViewModel.DTOs.Dashboard.DashboardDto AssignUrl(Zbang.Zbox.ViewModel.DTOs.Dashboard.DashboardDto data)
        {
            UrlBuilder builder = new UrlBuilder(this.HttpContext);
            foreach (var item in data.Boxes)
            {
                item.Url = builder.BuildBoxUrl(item.BoxType, item.Id, item.Name, item.UniName);
            }
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

        //[Ajax]
        //[HttpGet]
        //public ActionResult Side()
        //{
        //    var userid = GetUserId();
        //    try
        //    {
        //        var userDetail = m_FormsAuthenticationService.GetUserData();
        //        var universityWrapper = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;
        //        var query = new GetDashboardQuery(userid, universityWrapper);
        //        var data = m_ZboxReadService.GetMyData(query);
        //        return Json(new JsonResponse(true, data), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("BoxList user: {0}", userid), ex);
        //        return Json(new JsonResponse(false), JsonRequestBehavior.AllowGet);
        //    }
        //}

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
                 UserRelationshipType.Owner, 0, null,
                 0, 0, string.Empty, string.Empty, BoxType.Box, string.Empty,
                 builder.BuildBoxUrl(BoxType.Box, result.NewBox.Id, command.BoxName, string.Empty)
                 );
            return retVal;
        }


        #endregion

        #region search
        [HttpGet]
        [ActionName("Search")]
        [NonAjax]
        [UserNavNWelcome]
        [CompressFilter]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }
            var userid = GetUserId();
            var userDetail = m_FormsAuthenticationService.GetUserData();

            var universityWrapper = userDetail.UniversityWrapperId ?? userDetail.UniversityId.Value;
            //TODO - Combine to 1 tranaction
            //var dbquery = new GetDashboardQuery(userid, universityWrapper);
            //var data = await m_ZboxReadService.GetMyData(dbquery);



            var dbquery = new GetDashboardQuery(userid, universityWrapper);
            var taskData = m_ZboxReadService.GetMyData(dbquery);

            var queryboxes = new GetBoxesQuery(userid);
            var taskBoxes = m_ZboxReadService.GetDashboard(queryboxes);

            await Task.WhenAll(taskData, taskBoxes);
            var data = taskBoxes.Result;
            data = AssignUrl(data);

            JsonNetSerializer serializer = new JsonNetSerializer();

            ViewBag.Boxes = serializer.Serialize(data);

            if (Request.IsAjaxRequest())
            {
                return PartialView("Index", taskData.Result);
            }
            return View("Index", taskData.Result);


            // return View("Index", data);

        }
        [HttpGet, ActionName("Search")]
        [Ajax]
        public async Task<ActionResult> SearchAjax(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return this.CdJson(new JsonResponse(false, "need to have query"));
            }

            var userDetail = m_FormsAuthenticationService.GetUserData();

            if (!userDetail.UniversityId.HasValue)
            {
                return this.CdJson(new JsonResponse(false, Zbang.Cloudents.Mvc4WebRole.Controllers.Resources.LibraryControllerResources.LibraryController_Create_You_need_to_sign_up_for_university));
            }


            var searchQuery = new SearchLibraryDashBoardQuery(userDetail.UniversityId.Value, query, 0, GetUserId());
            // var searchQuery = searchFactory.GetQuery(query, 0, SearchType.Box, GetUserId());
            var result = await m_ZboxReadService.Search(searchQuery);
            var searchables = result as IList<BoxDto> ?? result.ToList();
            var urlBuilder = new UrlBuilder(HttpContext);
            return this.CdJson(new JsonResponse(true, new
            {
                boxes = searchables.Select(s =>
                {
                    s.Url = urlBuilder.BuildBoxUrl(s.Id, s.Name, s.UniName);
                    return s;
                })
            }));
        }

        #endregion search


    }
}
