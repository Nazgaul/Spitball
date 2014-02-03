using DevTrends.MvcDonutCaching;
using System;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Controllers.Resources;
using Zbang.Zbox.Mvc3WebRole.Factories;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Autocomplete;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxHandleError(ExceptionType = typeof(UnauthorizedAccessException), View = "Error")]
    public class BoxController : BaseController
    {
        public BoxController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }
        /// <summary>
        /// Page of box view
        /// </summary>
        /// <param name="BoxUid">The short id of box</param>
        /// <returns>box view</returns>
        [HttpGet]
        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Index(string BoxUid)
        {
            var userId = GetUserId(false);
            try
            {
                if (string.IsNullOrEmpty(BoxUid))
                    return RedirectToAction("Index", "Dashboard");
                ViewBag.boxid = BoxUid;
                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var query = new GetBoxQuery(boxId, userId);
                var result = m_ZboxReadService.GetBox2(query);
                return View("Index2", result);
            }
            catch (BoxAccessDeniedException)
            {
                return View("Error");
            }
            catch (BoxDoesntExistException)
            {
                return View("Error");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Index BoxUid {0} userid {1}", BoxUid, userId), ex);
                return View("Error");
            }
        }


        [HttpPost]
        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Items(string BoxUid, int pageNumber)
        {
            var userId = GetUserId(false); // not really needs it
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var query = new GetBoxItemsPagedQuery(boxId, userId);
                var result = m_ZboxReadService.GetBoxItemsPaged2(query);
                return Json(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxUid {0} pageNumber {1} userId {2}", BoxUid, pageNumber, userId), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpPost]
        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult Updates(string BoxUid, DateTime UserLastSync)
        {

            if (string.IsNullOrEmpty(BoxUid))
                return new HttpStatusCodeResult(500);
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId(false);

            var query = new GetBoxUpdatesQuery(boxId, userId, UserLastSync.AddSeconds(-30));
            var result = m_ZboxReadService.GetBoxUpdate(query);

            return Json(new JsonResponse(true, result));

        }



        [ChildActionOnly]
        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult BoxPermissionByUser(string BoxUid)
        {
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId(false);
            var permission = GetUserPermission(userId, boxId);
            if (!permission.CanUserSeeData)
            {
                throw new SecurityException(BoxControllerResources.BoxSettingsNotAllowed); // unathorized exception doesnt raise http 500 it seems
            }
            return PartialView("_BoxPermissionByUser", permission);
        }

        [ChildActionOnly]
        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult UserPermission(string BoxUid)
        {
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId(false);
            var permission = GetUserPermission(userId, boxId);
            if (!permission.CanUserSeeData)
            {
                throw new SecurityException(BoxControllerResources.BoxSettingsNotAllowed); // unathorized exception doesnt raise http 500 it seems
            }
            return PartialView("UserPermission", permission);
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Ajax, HttpPost]
        public JsonResult Members(string BoxUid)
        {
            if (string.IsNullOrWhiteSpace(BoxUid))
            {
                return Json(new JsonResponse(false));
            }
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId(false);
            var result = m_ZboxReadService.GetBoxMembers(new GetBoxQuery(boxId, userId));

            return Json(new JsonResponse(true, result));
        }

        [ZboxAuthorize]
        public ActionResult Settings(string BoxUid)
        {
            if (string.IsNullOrEmpty(BoxUid))
                throw new ArgumentException("Box id is missing");
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userId = GetUserId();

            var query = new GetBoxQuery(boxId, userId);
            var result = m_ZboxReadService.GetBoxSetting(query);

            var model = new BoxSetting()
            {
                Name = result.Name,
                Notification = result.NotificationSetting,
                Privacy = result.PrivacySetting
            };
            ViewBag.UserType = result.UserType;
            return PartialView("_Settings2", model);
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Settings(BoxSetting model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(model.Uid);
                var userId = GetUserId();
                if (model.Privacy.HasValue)
                {
                    ChangePrivacySettings(model.Privacy.Value, model.Uid, userId);
                }

                var commandBoxName = new ChangeBoxNameCommand(boxId, userId, model.Name);
                m_ZboxWriteService.ChangeBoxName(commandBoxName);

                var command = new ChangeNotificationSettingsCommand(boxId, userId, model.Notification);
                m_ZboxWriteService.ChangeNotificationSettings(command);


                return Json(new JsonResponse(true));
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new JsonResponse(false, "You don't have permission"));
            }
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

        //[HttpPost]
        //[ZboxAuthorize]
        //public ActionResult Tags(string term)
        //{
        //    var userId = GetUserId();
        //    var result = m_ZboxReadService.GetTagsByPrefix(new GetTagsQuery(userId, term));
        //    return Json(new JsonResponse(true, result));
        //}


        //[HttpPost]
        //[ZboxAuthorize]
        //public ActionResult SaveTag(string tagName, string BoxUid)
        //{
        //    if (string.IsNullOrWhiteSpace(BoxUid))
        //    {
        //        return Json(new JsonResponse(false, new { error = "BoxUid cannot be null" }));
        //    }
        //    if (string.IsNullOrWhiteSpace(tagName))
        //    {
        //        return Json(new JsonResponse(false, new { error = "tagName cannot be null" }));
        //    }

        //    var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
        //    var userId = GetUserId();
        //    m_ZboxWriteService.AddTagToBox(new AddTagToBoxCommand(Guid.NewGuid(), boxId, new[] { tagName }, userId));
        //    return Json(new JsonResponse(true));
        //}
        //[HttpPost]
        //[ZboxAuthorize]
        //public ActionResult DeleteTag(string tagName, string BoxUid)
        //{
        //    if (string.IsNullOrWhiteSpace(BoxUid))
        //    {
        //        return Json(new JsonResponse(false, new { error = "BoxUid cannot be null" }));
        //    }
        //    if (string.IsNullOrWhiteSpace(tagName))
        //    {
        //        return Json(new JsonResponse(false, new { error = "tagName cannot be null" }));
        //    }

        //    var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
        //    var userId = GetUserId();
        //    m_ZboxWriteService.DeleteTagFromBox(new DeleteTagFromBoxCommand(tagName, boxId, userId));
        //    return Json(new JsonResponse(true));
        //}



        /// <summary>
        /// Change box privacy settings - happen when user press on copy link
        /// </summary>
        /// <param name="boxUid"></param>
        /// <param name="privacy"></param>
        /// <returns></returns>
        [HttpPost]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public ActionResult ChangePrivacySettings(string boxUid, BoxPrivacySettings privacy = BoxPrivacySettings.AnyoneWithUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(boxUid))
                {
                    return Json(new JsonResponse(false));
                }
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
        private ChangeBoxPrivacySettingsCommandResult ChangePrivacySettings(BoxPrivacySettings privacy, string boxUid, long userId)
        {
            var boxId = m_ShortToLongCode.ShortCodeToLong(boxUid);

            var privacyCommand = new ChangeBoxPrivacySettingsCommand(userId, boxId, privacy, string.Empty);
            var privacyResult = m_ZboxWriteService.ChangeBoxPrivacySettings(privacyCommand);
            return privacyResult;
        }


        //[HttpGet]
        //[DonutOutputCache(Duration = 10 * TimeConsts.Minute, Location = OutputCacheLocation.Client, NoStore = true)]
        //[ZboxAuthorize]
        //public JsonResult Names(string term, BoxType? type)
        //{
        //    try
        //    {
        //        BoxTypeFactory factory = new BoxTypeFactory();
        //        GetBoxByNameQuery query = factory.GetBoxByNameQuery(type, term, GetUserId());
        //        var result = m_ZboxReadService.GetBoxNameIdByNamePrefix(query);
        //        return Json(new JsonResponse(true, result.Select(s => new { label = s.Name, value = s.Uid })), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("GetBoxName user {0}", User.Identity.Name), ex);
        //        return Json(new JsonResponse(false, "Problem with get user boxes by prefix"), JsonRequestBehavior.AllowGet);
        //    }
        //}


        [HttpGet]
        [DonutOutputCache(Duration = 10 * TimeConsts.Minute, Location = OutputCacheLocation.Client, NoStore = true)]
        [ZboxAuthorize]
        public JsonResult UploadBoxData(string BoxUid, BoxType? type)
        {
            //var userId = GetUserId();
            //BoxTypeFactory factory = new BoxTypeFactory();
            //var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            //var query = factory.GetUploadBoxDataQuery(type, boxId, userId);
            //var result = m_ZboxReadService.GetBoxDataForUpload(query);
            return Json(new JsonResponse(true, string.Empty), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeNotification(string BoxUid, NotificationSettings notification)
        {
            var userId = GetUserId();
            var boxId = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var command = new ChangeNotificationSettingsCommand(boxId, userId, notification);
            m_ZboxWriteService.ChangeNotificationSettings(command);
            return Json(new JsonResponse(true));

        }



    }
}
