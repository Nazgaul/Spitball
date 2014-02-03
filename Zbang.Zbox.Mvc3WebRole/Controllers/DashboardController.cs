using DevTrends.MvcDonutCaching;
using System;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Factories;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class DashboardController : BaseController
    {
        public DashboardController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }

        [ZboxAuthorize]
        [OutputCache(Duration = TimeConsts.Day, Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }
        /// <summary>
        /// View - boxes page
        /// </summary>
        /// <returns></returns>                
        //[ZboxAuthorize]
        //public ActionResult Index(string friendUid)
        //{
        //    var IsFriend = !string.IsNullOrEmpty(friendUid);
        //    var userid = string.IsNullOrWhiteSpace(friendUid) ? GetUserId() : ShortToLongCode.ShortCodeToLong(friendUid, ShortCodesType.User);
        //    if (IsFriend && userid == GetUserId())
        //    {
        //        return Redirect(Url.RouteUrl("Default"));//("Index", new { friendUid = string.Empty });
        //    }
        //    var userData = ZboxReadService.GetUserType(new GetUserTypeQuery(userid));

        //    ViewBag.IsFriend = IsFriend;
        //    return View("Index2", userData);
        //}

        //[ZboxAuthorize]
        //[ChildActionOnly]
        //public ActionResult MemberInfo(string friendUid)
        //{
        //    if (string.IsNullOrWhiteSpace(friendUid))
        //    {
        //        return new EmptyResult();
        //    }
        //    var friendId = m_ShortToLongCode.ShortCodeToLong(friendUid, ShortCodesType.User);
        //    var user = m_ZboxReadService.GetMemberDetail(new GetUserDetailsQuery(friendId));
        //    return PartialView("_MemberInfo", user);
        //}

        /// <summary>
        /// Box list in boxes view
        /// </summary>
        /// <returns></returns>
        [NoCache, Ajax]
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult BoxList(int pageNumber = 0)
        {
            var userid = GetUserId();

            try
            {
                var query = new GetBoxesQuery(userid, pageNumber, null, OrderBy.LastModified);
                var boxes = m_ZboxReadService.GetBoxes(query);

                return Json(new JsonResponse(true, new { boxes = boxes.Elem, count = boxes.Count }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("BoxList user: {0}", userid), ex);
                return new HttpStatusCodeResult(500);
            }
        }


        //[NoCache]
        //[HttpPost]
        //[ZboxAuthorize]
        //public ActionResult Wall()
        //{
        //    var userId = GetUserId();
        //    var query = new GetWallQuery(userId, 0);
        //    var result = m_ZboxReadService.GetWall(query);

        //    return Json(new JsonResponse(true, result));
        //}

        #region CreateBox

        [HttpPost]
        [ZboxAuthorize]
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
                var command = new CreateBoxCommand(userId, model.BoxName, model.Description);
                var retVal = CreateBox(command);
                return Json(new JsonResponse(true, retVal));

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("CreateNewBox user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, "Problem with Create new box");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }


        [NonAction]
        public BoxDto CreateBox(CreateBoxCommand command)
        {
            var result = m_ZboxWriteService.CreateBox(command);
            var shortBoxid = m_ShortToLongCode.LongToShortCode(result.NewBox.Id);
            //TODO: User name can come from cookie detail one well do that
            //var retVal = new BoxDto(shortBoxid, command.BoxName,
            //     DateTime.UtcNow, UserRelationshipType.Owner, 0, null,
            //     0, 0);
            return null;
        }

        //[HttpPost]
        //public ActionResult BoxNameDuplicate(CreateBox model)
        //{
        //    var userId = GetUserId();
        //    var query = new GetBoxNameExistsQuery(model.BoxName, userId);
        //    var result = ZboxReadService.GetBoxNameExists(query);
        //    return Json(!result);
        //}
        #endregion



        #region DeleteBox
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult DeleteBox(string BoxUid, UserRelationshipType UserType)
        {

            if (UserType == UserRelationshipType.Owner)
            {
                return DeleteOwnedBox(BoxUid);
            }
            if (UserType == UserRelationshipType.Subscribe)
            {
                var userId = GetUserId();
                return DeleteUserFomBox(BoxUid, userId);
            }
            return Json(new JsonResponse(false));

        }
        private ActionResult DeleteOwnedBox(string BoxUid)
        {
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var userId = GetUserId();
                var query = new DeleteBoxCommand(boxId, userId);
                m_ZboxWriteService.DeleteBox(query);
                return Json(new JsonResponse(true, BoxUid));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteBox user: {0} boxid: {1}", GetUserId(), BoxUid), ex);
                return Json(new JsonResponse(false));
            }
        }

        [NonAction]
        private ActionResult DeleteUserFomBox(string BoxUid, long userToDeleteId)
        {
            try
            {
                var userId = GetUserId();
                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var command = new DeleteUserFromBoxCommand(userId, userToDeleteId, boxId);
                m_ZboxWriteService.DeleteUserFromBox(command);

                //Check
                return Json(new JsonResponse(true, BoxUid));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteSubscription user: {0} boxid: {1}", GetUserId(), BoxUid), ex);
                return Json(new JsonResponse(false));
            }
        }
        #endregion


        /// <summary>
        /// Box Setting page
        /// </summary>
        /// <param name="BoxUid"></param>
        /// <param name="UserUid"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ZboxAuthorize]
        public ActionResult RemoveUser(string BoxUid, string UserUid)
        {
            var userToChangeId = m_ShortToLongCode.ShortCodeToLong(UserUid, ShortCodesType.User);
            return DeleteUserFomBox(BoxUid, userToChangeId);
        }

        /// <summary>
        /// Used by global serach to get template for boxes
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        [ZboxAuthorize]
        [DonutOutputCache(VaryByParam = "none", Duration = TimeConsts.Week, VaryByCustom = "Lang")]
        public ActionResult Template()
        {
            return PartialView("_Template");
        }


        /// <summary>
        /// Used in account settings notification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ZboxAuthorize]
        public JsonResult Notification()
        {
            var userid = GetUserId();
            var query = new GetUserDetailsQuery(userid);
            var result = m_ZboxReadService.GetUserBoxesNotification(query);

            return Json(new JsonResponse(true, result));
        }


    }
}
