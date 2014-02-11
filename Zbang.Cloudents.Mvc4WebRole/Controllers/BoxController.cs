﻿using DevTrends.MvcDonutCaching;
using System;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Models.Tabs;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.Domain;
using System.Web.WebPages;
using Zbang.Zbox.Infrastructure.Culture;
using System.Text.RegularExpressions;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxHandleError(ExceptionType = typeof(UnauthorizedAccessException), View = "Error")]
    [ZboxHandleError(ExceptionType = typeof(BoxAccessDeniedException), View = "Error")]
    [NoUniversity]
    public class BoxController : BaseController
    {
        private readonly Lazy<IShortCodesCache> m_ShortToLongCode;

        public BoxController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            Lazy<IShortCodesCache> shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        {
            m_ShortToLongCode = shortToLongCache;
        }


        /// <summary>
        /// Page of box view
        /// </summary>
        /// <param name="boxUid">The short id of box</param>
        /// <returns>box view</returns>
        [NonAjax]
        [Route("box/{boxUid:length(11)}")]
        public ActionResult Index(string boxUid)
        {
            try
            {
                var boxid = m_ShortToLongCode.Value.ShortCodeToLong(boxUid);
                var query = new GetBoxQuery(boxid, GetUserId(false));
                var box = m_ZboxReadService.GetBox2(query);

                UrlBuilder builder = new UrlBuilder(HttpContext);
                var url = builder.BuildBoxUrl(box.BoxType, boxid, box.Name, box.OwnerName);
                return RedirectPermanent(url);
            }
            catch (BoxAccessDeniedException)
            {
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }
                return RedirectToAction("MembersOnly", "Error");
            }
            catch (BoxDoesntExistException)
            {
                if (Request.IsAjaxRequest())
                {
                    return HttpNotFound();
                }
                return RedirectToAction("Index", "Error");
            }
        }

        [CacheFilter(Duration = 0)]
        [CompressFilter]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [UserNavNWelcome]
        [AjaxCache(TimeConsts.Minute * 10)]
        [Route("box/my/{boxId:long}/{boxName}", Name = "PrivateBox")]
        [Route("course/{universityName}/{boxId:long}/{boxName}", Name = "CourseBox")]
        public ActionResult Index(string universityName, long boxId, string boxName)
        {
            var userId = GetUserId(false);
            try
            {
                var query = new GetBoxQuery(boxId, userId);
                var box = m_ZboxReadService.GetBox2(query);


                if (box.BoxType == BoxType.Academic && !string.IsNullOrEmpty(box.UniCountry))
                {
                    var culture = Languages.GetCultureBaseOnCountry(box.UniCountry);
                    BaseControllerResources.Culture = culture;
                    ViewBag.title = string.Format("{0} {1} | {2} | Cloudents", BaseControllerResources.TitlePrefix, box.Name, box.OwnerName);
                    ViewBag.metaDescription = Regex.Replace(string.Format(
                        BaseControllerResources.MetaDescription, box.Name,
                        string.IsNullOrWhiteSpace(box.CourseId) ? string.Empty : string.Format(", #{0}", box.CourseId),
                        string.IsNullOrWhiteSpace(box.ProfessorName) ? string.Empty : string.Format("{0} {1}", BaseControllerResources.MetaDescriptionBy, box.ProfessorName)), @"\s+", " ");
                }
                else
                {
                    ViewBag.title = string.Format("{0} | Cloudents", box.Name);
                }

                var urlBuilder = new UrlBuilder(HttpContext);
                if (boxName != UrlBuilder.NameToQueryString(box.Name))
                {
                    throw new BoxDoesntExistException();
                }
                box.Subscribers = box.Subscribers.Select(s =>
                {
                    s.Url = urlBuilder.BuildUserUrl(s.Uid, s.Name);
                    return s;
                });
                JsonNetSerializer serializer = new JsonNetSerializer();
                ViewBag.data = serializer.Serialize(box);

                ViewBag.boxid = boxId;
                if (Request.IsAjaxRequest())
                {
                    return PartialView();
                }
                return View();
            }
            catch (BoxAccessDeniedException)
            {
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }
                return RedirectToAction("MembersOnly", "Error");
            }
            catch (BoxDoesntExistException)
            {
                if (Request.IsAjaxRequest())
                {
                    return HttpNotFound();
                }
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Index boxId {0} userid {1}", boxId, userId), ex);
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
                }
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpGet, Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [AjaxCache(TimeConsts.Minute * 10)]
        public ActionResult Data(long boxUid)
        {
            var userId = GetUserId(false);
            try
            {

                var query = new GetBoxQuery(boxUid, userId);
                var result = m_ZboxReadService.GetBox2(query);
                var urlBuilder = new UrlBuilder(HttpContext);
                result.Subscribers = result.Subscribers.Select(s =>
                {
                    s.Url = urlBuilder.BuildUserUrl(s.Uid, s.Name);
                    return s;
                });
                return this.CdJson(new JsonResponse(true, result));
            }
            catch (BoxAccessDeniedException)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            catch (BoxDoesntExistException)
            {
                return HttpNotFound();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Index BoxUid {0} userid {1}", boxUid, userId), ex);
                return this.CdJson(new JsonResponse(false));
            }
        }


        [HttpGet]
        [Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [AjaxCache(TimeConsts.Minute * 10)]
        public ActionResult Items(long boxUid, int pageNumber, Guid? tab, string uniName, string boxName)
        {
            var userId = GetUserId(false); // not really needs it
            try
            {
                var query = new GetBoxItemsPagedQuery(boxUid, userId, pageNumber, OrderBy.LastModified, tab);
                var result = m_ZboxReadService.GetBoxItemsPaged2(query);
                var urlBuilder = new UrlBuilder(HttpContext);
                foreach (var item in result.Dto)
                {
                    item.Url = urlBuilder.buildItemUrl(boxUid, boxName, item.Id, item.Name, uniName);
                }

                return this.CdJson(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxUid {0} pageNumber {1} userId {2}", boxUid, pageNumber, userId), ex);
                return this.CdJson(new JsonResponse(false));
            }
        }









        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Ajax, HttpGet]
        [AjaxCache(TimeToCache = TimeConsts.Minute * 15)]
        public JsonResult Members(long boxUid)
        {
            var userId = GetUserId(false);
            var result = m_ZboxReadService.GetBoxMembers(new GetBoxQuery(boxUid, userId));
            var urlBuilder = new UrlBuilder(HttpContext);
            result = result.Select(s =>
            {
                s.Url = urlBuilder.BuildUserUrl(s.Uid, s.Name);
                return s;
            });
            return Json(new JsonResponse(true, result), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Used in mobile - box settings page
        /// </summary>
        /// <param name="boxUid"></param>
        /// <returns></returns>
        [ZboxAuthorize]
        public ActionResult Settings(long boxUid)
        {

            var userId = GetUserId();

            var query = new GetBoxQuery(boxUid, userId);
            var result = m_ZboxReadService.GetBoxSetting(query);

            var model = new BoxSetting
              {
                  Name = result.Name,
                  Notification = result.NotificationSetting,
                  Privacy = result.PrivacySetting,
                  UserType = result.UserType
              };
            return View(model);
        }



        [HttpPost]
        [ZboxAuthorize]
        [Ajax]
        public ActionResult UpdateInfo(UpdateBoxInfo model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = GetUserId();
            try
            {
                var commandBoxName = new ChangeBoxInfoCommand(model.BoxUid, userId, model.Name,
                    model.Professor, model.CourseCode, model.Picture, model.BoxPrivacy, model.Notification);
                m_ZboxWriteService.ChangeBoxInfo(commandBoxName);
                // ChangeNotification(model.BoxUid, model.Notification);
                return Json(new JsonResponse(true));
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new JsonResponse(false, "You don't have permission"));
            }
            catch (ArgumentException)
            {
                return Json(new JsonResponse(false));
            }
        }

        /// <summary>
        /// Change box privacy settings - happen when user press on copy link
        /// </summary>
        /// <param name="boxUid"></param>
        /// <param name="privacy"></param>
        /// <returns></returns>
        [HttpPost, Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult ChangePrivacySettings(long boxUid, BoxPrivacySettings privacy = BoxPrivacySettings.AnyoneWithUrl)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new JsonResponse(false));
                }

                var userId = GetUserId();
                var result = ChangePrivacySettings(privacy, boxUid, userId);

                return Json(new JsonResponse(true, result.PrivacyChanged));
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new JsonResponse(false));
            }

        }

        [NonAction]
        private ChangeBoxPrivacySettingsCommandResult ChangePrivacySettings(BoxPrivacySettings privacy, long boxUid, long userId)
        {

            var privacyCommand = new ChangeBoxPrivacySettingsCommand(userId, boxUid, privacy, string.Empty);
            var privacyResult = m_ZboxWriteService.ChangeBoxPrivacySettings(privacyCommand);
            return privacyResult;
        }




        [ZboxAuthorize]
        [HttpGet]
        [Ajax]
        public JsonResult GetNotification(long boxUid)
        {
            var userId = GetUserId();

            var result = m_ZboxReadService.GetUserBoxNotificationSettings(new GetBoxQuery(boxUid, userId));
            return Json(new JsonResponse(true, result.ToString("g")), JsonRequestBehavior.AllowGet);
        }

        [ZboxAuthorize]
        [HttpPost]
        [Ajax]
        [AjaxCache(TimeToCache = TimeConsts.Hour)]
        public JsonResult ChangeNotification(long boxUid, NotificationSettings notification)
        {
            var userId = GetUserId();
            var command = new ChangeNotificationSettingsCommand(boxUid, userId, notification);
            m_ZboxWriteService.ChangeNotificationSettings(command);
            return Json(new JsonResponse(true));

        }

        #region DeleteBox
        [HttpPost]
        [Ajax]
        [ZboxAuthorize]
        [Obsolete]
        public ActionResult Delete(long boxUid)
        {
            var userId = GetUserId();

            return DeleteOwnedBox(boxUid, userId);


        }

        [HttpPost]
        [Ajax]
        [ZboxAuthorize]
        public ActionResult Delete2(long boxUid)
        {
            var userId = GetUserId();
            var command = new UnfollowBoxCommand(boxUid, userId);
            m_ZboxWriteService.UnfollowBox(command);
            return Json(new JsonResponse(true));
        }

        [HttpPost]
        [Ajax]
        [ZboxAuthorize]
        [Obsolete]
        public ActionResult Unfollow(long boxUid)
        {
            return DeleteUserFomBox(boxUid, GetUserId());
        }
        [NonAction]
        private ActionResult DeleteOwnedBox(long boxId, long userId)
        {
            try
            {

                var query = new DeleteBoxCommand(boxId, userId);
                m_ZboxWriteService.DeleteBox(query);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteBox user: {0} boxid: {1}", GetUserId(), boxId), ex);
                return Json(new JsonResponse(false));
            }
        }

        [NonAction]
        private ActionResult DeleteUserFomBox(long boxId, long userToDeleteId)
        {
            try
            {
                var userId = GetUserId();
                var command = new DeleteUserFromBoxCommand(userId, userToDeleteId, boxId);
                m_ZboxWriteService.DeleteUserFromBox(command);

                //Check
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteSubscription user: {0} boxid: {1}", GetUserId(), boxId), ex);
                return Json(new JsonResponse(false));
            }
        }

        /// <summary>
        /// Box Setting page
        /// </summary>
        /// <param name="boxUid"></param>
        /// <param name="userUid"></param>
        /// <returns></returns>
        /// 
        [HttpPost, Ajax]
        [ZboxAuthorize]
        public ActionResult RemoveUser(long boxUid, long userId)
        {
            // var userToChangeId = m_ShortToLongCode.ShortCodeToLong(userUid, ShortCodesType.User);
            return DeleteUserFomBox(boxUid, userId);
        }
        #endregion




        #region Tab


        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult CreateTab(CreateBoxItemTab model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var userId = GetUserId();
                var guid = Guid.NewGuid();
                var command = new CreateItemTabCommand(guid, model.Name, model.BoxId, userId);
                m_ZboxWriteService.CreateBoxItemTab(command);
                var result = new TabDto { Id = guid, Name = model.Name };
                return this.CdJson(new JsonResponse(true, result));
            }
            catch (ArgumentException ex)
            {
                return this.CdJson(new JsonResponse(false, ex.Message));
            }

        }

        [HttpPost, Ajax, ZboxAuthorize]
        public JsonResult AddItemToTab(AssignItemToTab model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            model.ItemId = model.ItemId ?? new long[0];
            var userId = GetUserId();
            var command = new AssignItemToTabCommand(model.ItemId, model.TabId, model.BoxId, userId, model.nDelete);
            m_ZboxWriteService.AssignBoxItemToTab(command);
            return Json(new JsonResponse(true));
        }

        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult RenameTab(ChangeBoxItemTabName model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = GetUserId();
            var command = new ChangeItemTabNameCommand(model.TabId, model.NewName, userId, model.BoxUid);
            m_ZboxWriteService.RenameBoxItemTab(command);
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax, ZboxAuthorize]
        public JsonResult DeleteTab(DeleteBoxItemTab model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = GetUserId();
            var command = new DeleteItemTabCommand(userId, model.TabId, model.BoxUid);
            m_ZboxWriteService.DeleteBoxItemTab(command);
            return Json(new JsonResponse(true));
        }
        #endregion

    }
}
