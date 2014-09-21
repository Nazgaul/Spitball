using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.Mvc4WebRole.Models.Tabs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Dto;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Queries;
using ItemDto = Zbang.Zbox.ViewModel.Dto.ItemDtos.ItemDto;

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
        public async Task<ActionResult> Index(string boxUid)
        {
            try
            {
                var boxid = m_ShortCodesCache.Value.ShortCodeToLong(boxUid);
                var query = new GetBoxSeoQuery(boxid);
                var model = await ZboxReadService.GetBoxSeo(query);
                if (model == null)
                {
                    throw new BoxDoesntExistException();
                }
                return RedirectPermanent(model.Url);
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
        [NoCache]
        public async Task<ActionResult> IndexDesktop(string universityName, long boxId, string boxName)
        {
            var userId = GetUserId(false);
            try
            {
                var query = new GetBoxSeoQuery(boxId);
                var model = await ZboxReadService.GetBoxSeo(query);
                if (model == null)
                {
                    throw new BoxDoesntExistException();
                }
                if (Request.Url != null && model.Url != Server.UrlDecode(Request.Url.AbsolutePath))
                {
                    throw new ItemNotFoundException();
                }
                if (model.BoxType == BoxType.Box)
                {
                    ViewBag.title = string.Format("{0} | {1}", model.Name, BaseControllerResources.Cloudents);
                    return View("Empty");
                }
                BaseControllerResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
                ViewBag.title = string.Format("{0} {1} | {2} | {3}", BaseControllerResources.TitlePrefix, model.Name,
                    model.UniversityName, BaseControllerResources.Cloudents);
                ViewBag.metaDescription = Regex.Replace(string.Format(
                    BaseControllerResources.MetaDescription, model.Name,
                    string.IsNullOrWhiteSpace(model.CourseId) ? string.Empty : string.Format(", #{0}", model.CourseId),
                    string.IsNullOrWhiteSpace(model.Professor)
                        ? string.Empty
                        : string.Format("{0} {1}", BaseControllerResources.MetaDescriptionBy, model.Professor)),
                    @"\s+", " ");
                return View("Empty");
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

        [Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        //[DonutOutputCache(Duration = TimeConsts.Minute * 5,
        //    Location = System.Web.UI.OutputCacheLocation.ServerAndClient,
        //    VaryByCustom = CustomCacheKeys.Lang, Options = OutputCacheOptions.IgnoreQueryString, VaryByParam = "none")]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index");
        }

        [NoCache]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
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

                var serialize = new JsonNetSerializer();
                ViewBag.data = serialize.Serialize(box);

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
                return Request.Url == null ? RedirectToAction("MembersOnly", "Error")
                    : RedirectToAction("MembersOnly", "Error", new { returnUrl = Request.Url.AbsolutePath });
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
        public async Task<ActionResult> Data(long id)
        {
            var userId = GetUserId(false);
            try
            {
                var query = new GetBoxQuery(id, userId);
                var result = await ZboxReadService.GetBox2(query);
                return Json(new JsonResponse(true, result));
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
                return Json(new JsonResponse(false));
            }
        }

        //TODO:we want to bring tab id as well so filter will be on client side
        [HttpGet]
        [Ajax]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public JsonResult Items(long id, Guid? tab)
        {
            var userId = GetUserId(false); // not really needs it
            try
            {
                var query = new GetBoxItemsPagedQuery(id, userId,  tab);
                var result = ZboxReadService.GetBoxItemsPaged2(query);
                var urlBuilder = new UrlBuilder(HttpContext);
                var itemDtos = result as IList<IItemDto> ?? result.ToList();
                foreach (var item in itemDtos)
                {
                    item.DownloadUrl = urlBuilder.BuildDownloadUrl(id, item.Id);
                }
                var remove = itemDtos.OfType<QuizDto>().Where(w => !w.Publish && w.OwnerId != GetUserId(false));
                return Json(new JsonResponse(true, itemDtos.Except(remove).OrderByDescending(o => o.Date)));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxUid {0} userId {1}", id, userId), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpGet,Ajax,ZboxAuthorize(IsAuthenticationRequired = false)]
        public JsonResult Quizes(long id)
        {
            var userId = GetUserId(false); // not really needs it
            try
            {
                var query = new GetBoxItemsPagedQuery(id, userId);
                var result = ZboxReadService.GetBoxQuizes(query);
                var itemDtos = result as IList<IItemDto> ?? result.ToList();

                var remove = itemDtos.OfType<QuizDto>().Where(w => !w.Publish && w.OwnerId != GetUserId(false));
                return Json(new JsonResponse(true, itemDtos.Except(remove).OrderByDescending(o => o.Date)));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxUid {0} userId {1}", id,  userId), ex);
                return Json(new JsonResponse(false));
            }
        }









        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [Ajax, HttpGet]
        //[AjaxCache(TimeToCache = TimeConsts.Minute * 15)]
        public JsonResult Members(long boxId)
        {
            var userId = GetUserId(false);
            var result = ZboxReadService.GetBoxMembers(new GetBoxQuery(boxId, userId));
            return Json(new JsonResponse(true, result));
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
        public JsonResult UpdateInfo(UpdateBoxInfo model)
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
                ZboxWriteService.ChangeBoxInfo(commandBoxName);
                // ChangeNotification(model.BoxUid, model.Notification);
                return Json(new JsonResponse(true, new { queryString = UrlBuilder.NameToQueryString(model.Name) }));
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new JsonResponse(false, "You don't have permission"));
            }
            catch (ArgumentException)
            {
                return Json(new JsonResponse(false));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("on UpdateBox info model: {0} userid {1}", model, GetUserId()), ex);
                return Json(new JsonResponse(false));
            }
        }

        [ZboxAuthorize]
        [HttpGet]
        [Ajax]
        public JsonResult GetNotification(long boxId)
        {
            var userId = GetUserId();

            var result = ZboxReadService.GetUserBoxNotificationSettings(new GetBoxQuery(boxId, userId));
            return Json(new JsonResponse(true, result.ToString("g")));
        }

        [ZboxAuthorize]
        [HttpPost]
        [Ajax]
        public JsonResult ChangeNotification(long boxUid, NotificationSettings notification)
        {
            var userId = GetUserId();
            var command = new ChangeNotificationSettingsCommand(boxUid, userId, notification);
            ZboxWriteService.ChangeNotificationSettings(command);
            return Json(new JsonResponse(true));

        }


        [HttpGet, Ajax]
        // TODO: we need to put output cache with boxid
        //[OutputCache(Duration = TimeConsts.Hour, Location = OutputCacheLocation.Any,
        //    VaryByParam = "none", VaryByCustom = CustomCacheKeys.Lang)]

        public ActionResult SettingsPartial()
        {
            try
            {
                return PartialView("_BoxSettings");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_BoxSettings", ex);
                return Json(new JsonResponse(false));
            }
        }

        #region DeleteBox
        [HttpPost]
        [Ajax]
        [ZboxAuthorize]
        [Obsolete]
        public ActionResult Delete(long boxUid)
        {
            return Delete2(boxUid);


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

        //[HttpPost]
        //[Ajax]
        //[ZboxAuthorize]
        //[Obsolete]
        //public ActionResult Unfollow(long boxUid)
        //{
        //    return DeleteUserFomBox(boxUid, GetUserId());
        //}

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
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var userId = GetUserId();
                var guid = Guid.NewGuid();
                var command = new CreateItemTabCommand(guid, model.Name, model.BoxId, userId);
                ZboxWriteService.CreateBoxItemTab(command);
                var result = new TabDto { Id = guid, Name = model.Name };
                return Json(new JsonResponse(true, result));
            }
            catch (ArgumentException ex)
            {
                return Json(new JsonResponse(false, ex.Message));
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
            var command = new AssignItemToTabCommand(model.ItemId, model.TabId, model.BoxId, userId, model.NDelete);
            ZboxWriteService.AssignBoxItemToTab(command);
            return Json(new JsonResponse(true));
        }

        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult RenameTab(ChangeBoxItemTabName model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = GetUserId();
            var command = new ChangeItemTabNameCommand(model.TabId, model.Name, userId, model.BoxId);
            ZboxWriteService.RenameBoxItemTab(command);
            return Json(new JsonResponse(true));
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
                return Json(new JsonResponse(false));
            }
        }
        #endregion

        [ZboxAuthorize]
        [HttpPost]
        [Ajax]
        public JsonResult DeleteUpdates(long boxId)
        {
            var userId = GetUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            ZboxWriteService.DeleteUpdates(command);
            return Json(new JsonResponse(true));
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
                return Json(new JsonResponse(false));
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
                return Json(new JsonResponse(false));
            }
        }

    }
}
