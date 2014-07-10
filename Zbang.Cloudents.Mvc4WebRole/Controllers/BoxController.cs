using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.Mvc4WebRole.Models.Tabs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxHandleError(ExceptionType = typeof(UnauthorizedAccessException), View = "Error")]
    [ZboxHandleError(ExceptionType = typeof(BoxAccessDeniedException), View = "Error")]
    [NoUniversity]
    public class BoxController : BaseController
    {
        private readonly Lazy<IShortCodesCache> m_ShortCodesCache;
        public BoxController(
            Lazy<IShortCodesCache> shortToLongCache)
        {
            m_ShortCodesCache = shortToLongCache;
        }


        /// <summary>
        /// We need to keep it because of invite links 
        /// </summary>
        /// <returns>box view</returns>
        [NonAjax]
        [Route("box/{boxUid:length(11)}")]
        public ActionResult Index(string boxUid)
        {
            try
            {
                var boxid = m_ShortCodesCache.Value.ShortCodeToLong(boxUid);
                var query = new GetBoxQuery(boxid, GetUserId(false));
                var box = ZboxReadService.GetBox(query);

                var builder = new UrlBuilder(HttpContext);
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

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [UserNavNWelcome]
        [OutputCache(CacheProfile = "NoCache")]
        public ActionResult IndexDesktop(string universityName, long boxId, string boxName)
        {
            var userId = GetUserId(false);
            try
            {
                
                var query = new GetBoxQuery(boxId, userId);
                var box = ZboxReadService.GetBox(query);
                if (box.BoxType == BoxType.Academic && !string.IsNullOrEmpty(box.UniCountry))
                {

                    ViewBag.title = string.Format("{0} {1} | {2} | {3}", BaseControllerResources.TitlePrefix, box.Name,
                        box.OwnerName, BaseControllerResources.Cloudents);
                    ViewBag.metaDescription = Regex.Replace(string.Format(
                        BaseControllerResources.MetaDescription, box.Name,
                        string.IsNullOrWhiteSpace(box.CourseId) ? string.Empty : string.Format(", #{0}", box.CourseId),
                        string.IsNullOrWhiteSpace(box.ProfessorName)
                            ? string.Empty
                            : string.Format("{0} {1}", BaseControllerResources.MetaDescriptionBy, box.ProfessorName)),
                        @"\s+", " ");
                }
                else
                {
                    ViewBag.title = string.Format("{0} | {1}", box.Name, BaseControllerResources.Cloudents);
                }
                return View("Empty");
            }
            catch (BoxAccessDeniedException)
            {
                return RedirectToAction("MembersOnly", "Error", new { returnUrl = Request.Url.AbsolutePath });
            }
            catch (BoxDoesntExistException)
            {
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Index boxId {0} userid {1}", boxId, userId), ex);
                return RedirectToAction("Index", "Error");
            }
        }

        [CacheFilter(Duration = 0)]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [UserNavNWelcome]
        public ActionResult Index(string universityName, long boxId, string boxName)
        {
            var userId = GetUserId(false);
            try
            {
                var query = new GetBoxQuery(boxId, userId);
                var box = ZboxReadService.GetBox(query);

                var culture = Languages.GetCultureBaseOnCountry(box.UniCountry);
                BaseControllerResources.Culture = culture;
                if (box.BoxType == BoxType.Academic && !string.IsNullOrEmpty(box.UniCountry))
                {

                    ViewBag.title = string.Format("{0} {1} | {2} | {3}", BaseControllerResources.TitlePrefix, box.Name, box.OwnerName, BaseControllerResources.Cloudents);
                    ViewBag.metaDescription = Regex.Replace(string.Format(
                        BaseControllerResources.MetaDescription, box.Name,
                        string.IsNullOrWhiteSpace(box.CourseId) ? string.Empty : string.Format(", #{0}", box.CourseId),
                        string.IsNullOrWhiteSpace(box.ProfessorName) ? string.Empty : string.Format("{0} {1}", BaseControllerResources.MetaDescriptionBy, box.ProfessorName)), @"\s+", " ");
                }
                else
                {
                    ViewBag.title = string.Format("{0} | {1}", box.Name, BaseControllerResources.Cloudents);
                }

                if (boxName != UrlBuilder.NameToQueryString(box.Name))
                {
                    throw new BoxDoesntExistException();
                }

                var serializer = new JsonNetSerializer();
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
                return RedirectToAction("MembersOnly", "Error", new { returnUrl = Request.Url.AbsolutePath });
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
        public ActionResult Data(long id)
        {
            var userId = GetUserId(false);
            try
            {

                var query = new GetBoxQuery(id, userId);
                var result = ZboxReadService.GetBox(query);
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
                TraceLog.WriteError(string.Format("Box Index BoxUid {0} userid {1}", id, userId), ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        //TODO: need to remove uni name and boxname once we got url from db we want to bring tab id as well so filter will be on client side
        [HttpGet]
        [Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        //[AjaxCache(TimeConsts.Minute * 10)]
        public JsonNetResult Items(long id, int pageNumber, Guid? tab)
        {
            var userId = GetUserId(false); // not really needs it
            try
            {
                var query = new GetBoxItemsPagedQuery(id, userId, pageNumber, OrderBy.LastModified, tab);
                var result = ZboxReadService.GetBoxItemsPaged2(query);
                var urlBuilder = new UrlBuilder(HttpContext);
                var itemDtos = result as IList<IItemDto> ?? result.ToList();
                foreach (var item in itemDtos)
                {
                    if (item is Zbox.ViewModel.DTOs.ItemDtos.ItemDto)
                    {
                        item.DownloadUrl = urlBuilder.BuildDownloadUrl(id, item.Id);
                    }

                }
                var remove = itemDtos.OfType<QuizDto>().Where(w => !w.Publish && w.OwnerId != GetUserId(false));
                return this.CdJson(new JsonResponse(true, itemDtos.Except(remove).OrderByDescending(o => o.Date)));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxUid {0} pageNumber {1} userId {2}", id, pageNumber, userId), ex);
                return this.CdJson(new JsonResponse(false));
            }
        }









        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Ajax, HttpGet]
        //[AjaxCache(TimeToCache = TimeConsts.Minute * 15)]
        public JsonNetResult Members(long boxUid)
        {
            var userId = GetUserId(false);
            var result = ZboxReadService.GetBoxMembers(new GetBoxQuery(boxUid, userId));
            return this.CdJson(new JsonResponse(true, result));
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
            var result = ZboxReadService.GetBoxSetting(query);

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
        public JsonNetResult UpdateInfo(UpdateBoxInfo model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = GetUserId();
            try
            {
                var commandBoxName = new ChangeBoxInfoCommand(model.BoxUid, userId, model.Name,
                    model.Professor, model.CourseCode, model.Picture, model.BoxPrivacy, model.Notification);
                ZboxWriteService.ChangeBoxInfo(commandBoxName);
                // ChangeNotification(model.BoxUid, model.Notification);
                return this.CdJson(new JsonResponse(true, new { queryString = UrlBuilder.NameToQueryString(model.Name) }));
            }
            catch (UnauthorizedAccessException)
            {
                return this.CdJson(new JsonResponse(false, "You don't have permission"));
            }
            catch (ArgumentException)
            {
                return this.CdJson(new JsonResponse(false));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("on UpdateBox info model: {0} userid {1}", model, GetUserId()), ex);
                return this.CdJson(new JsonResponse(false));
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
        public JsonNetResult ChangePrivacySettings(long boxUid, BoxPrivacySettings privacy = BoxPrivacySettings.AnyoneWithUrl)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return this.CdJson(new JsonResponse(false));
                }

                var userId = GetUserId();
                var result = ChangePrivacySettings(privacy, boxUid, userId);

                return this.CdJson(new JsonResponse(true, result.PrivacyChanged));
            }
            catch (UnauthorizedAccessException)
            {
                return this.CdJson(new JsonResponse(false));
            }

        }

        [NonAction]
        private ChangeBoxPrivacySettingsCommandResult ChangePrivacySettings(BoxPrivacySettings privacy, long boxUid, long userId)
        {

            var privacyCommand = new ChangeBoxPrivacySettingsCommand(userId, boxUid, privacy, string.Empty);
            var privacyResult = ZboxWriteService.ChangeBoxPrivacySettings(privacyCommand);
            return privacyResult;
        }




        [ZboxAuthorize]
        [HttpGet]
        [Ajax]
        public JsonNetResult GetNotification(long boxUid)
        {
            var userId = GetUserId();

            var result = ZboxReadService.GetUserBoxNotificationSettings(new GetBoxQuery(boxUid, userId));
            return this.CdJson(new JsonResponse(true, result.ToString("g")));
        }

        [ZboxAuthorize]
        [HttpPost]
        [Ajax]
        public JsonNetResult ChangeNotification(long boxUid, NotificationSettings notification)
        {
            var userId = GetUserId();
            var command = new ChangeNotificationSettingsCommand(boxUid, userId, notification);
            ZboxWriteService.ChangeNotificationSettings(command);
            return this.CdJson(new JsonResponse(true));

        }


        [HttpGet, Ajax]
        [OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any, VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]

        public ActionResult SettingsPartial()
        {
            try
            {
                return PartialView("_BoxSettings");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_BoxSettings", ex);
                return this.CdJson(new JsonResponse(false));
            }
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
        public ActionResult Delete2(long id)
        {
            var userId = GetUserId();
            var command = new UnfollowBoxCommand(id, userId);
            ZboxWriteService.UnfollowBox(command);
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
                ZboxWriteService.DeleteBox(query);
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
                ZboxWriteService.DeleteUserFromBox(command);

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
        /// <param name="userId"></param>
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
                ZboxWriteService.CreateBoxItemTab(command);
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
            ZboxWriteService.AssignBoxItemToTab(command);
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
            var command = new ChangeItemTabNameCommand(model.TabId, model.Name, userId, model.BoxId);
            ZboxWriteService.RenameBoxItemTab(command);
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
            var command = new DeleteItemTabCommand(userId, model.TabId, model.BoxId);
            ZboxWriteService.DeleteBoxItemTab(command);
            return Json(new JsonResponse(true));
        }
    
        [HttpGet, Ajax]
        [OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any, VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]
        public ActionResult CreateTabPartial()
        {
            try
            {
                return PartialView("_CreateTab");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_CreateTab ", ex);
                return this.CdJson(new JsonResponse(false));
            }
        }
        #endregion

        [ZboxAuthorize]
        [HttpPost]
        [Ajax]
        public JsonNetResult DeleteUpdates(long boxId)
        {
            var userId = GetUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            ZboxWriteService.DeleteUpdates(command);
            return this.CdJson(new JsonResponse(true));
        }

        [HttpGet, Ajax]
        [OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any, VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]

        public ActionResult UploadPartial()
        {
            try
            {
                return PartialView("_UploadDialog");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_UploadDialog ", ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

        [HttpGet, Ajax]
        [OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any, VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]

        public ActionResult UploadLinkPartial()
        {
            try
            {
                return PartialView("_UploadAddLink");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_UploadAddLink", ex);
                return this.CdJson(new JsonResponse(false));
            }
        }

    }       
}
