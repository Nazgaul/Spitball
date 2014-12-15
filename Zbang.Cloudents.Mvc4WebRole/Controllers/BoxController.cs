using System;
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

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxHandleError(ExceptionType = typeof(UnauthorizedAccessException), View = "Error")]
    [ZboxHandleError(ExceptionType = typeof(BoxAccessDeniedException), View = "Error")]
    [NoUniversity]
    public class BoxController : BaseController
    {
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(VaryByCustom = CustomCacheKeys.Lang,
           Duration = TimeConsts.Hour * 1, VaryByParam = "boxId",
           Location = OutputCacheLocation.Server, Order = 4)]
        [BoxPermission("boxId", Order = 3)]
        [PreserveQueryString(Order = 2)]
        [RedirectToMobile(Order = 1)]
        public async Task<ActionResult> Index(long boxId)
        {
            var userId = User.GetUserId(false);
            try
            {
                var query = new GetBoxSeoQuery(boxId, userId);
                var model = await ZboxReadService.GetBoxSeo(query);
                if (model == null)
                {
                    throw new BoxDoesntExistException("model is null");
                }
                if (Request.Url != null && model.Url != Server.UrlDecode(Request.Url.AbsolutePath))
                {
                    throw new BoxDoesntExistException(Request.Url.AbsoluteUri);
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
            catch (BoxAccessDeniedException)
            {
                return Request.Url == null ? RedirectToAction("MembersOnly", "Error")
                    : RedirectToAction("MembersOnly", "Error", new { returnUrl = Request.Url.AbsolutePath });
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


        [PreserveQueryString]
        public async Task<RedirectResult> ShortUrl(string box62Id)
        {
            var base62 = new Base62(box62Id);
            var userId = User.GetUserId(false);
            var query = new GetBoxSeoQuery(base62.Value, userId);
            var model = await ZboxReadService.GetBoxSeo(query);
            return RedirectPermanent(model.Url);

        }
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage",
            Options = OutputCacheOptions.IgnoreQueryString
            )]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index");
        }



        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<ActionResult> Data(long id)
        {
            var userId = User.GetUserId(false);
            try
            {
                var query = new GetBoxQuery(id, userId);
                var result = await ZboxReadService.GetBox2(query);
                result.UserType = ViewBag.UserType;
                result.ShortUrl = Url.RouteUrl("shortBox", new { box62Id = new Base62(id).ToString() });
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
                TraceLog.WriteError(string.Format("Box Index id {0} userid {1}", id, userId), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> SideBar(long id)
        {
            var query = new GetBoxSideBarQuery(id, User.GetUserId(false));
            var result = await ZboxReadService.GetBoxSideBar(query);
            return Json(new JsonResponse(true, result));
        }

        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Tabs(long id)
        {
            var userId = User.GetUserId(false);
            try
            {
                var query = new GetBoxQuery(id, userId);
                var result = await ZboxReadService.GetBoxTabs(query);
                return Json(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Tabs id {0} userid {1}", id, userId), ex);
                return Json(new JsonResponse(false));
            }
        }


        //TODO: change to box permission with dapper
        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public JsonResult Items(long id)
        {
            var userId = User.GetUserId(false);// not really needs it
            try
            {
                var query = new GetBoxItemsPagedQuery(id, userId);
                var result = ZboxReadService.GetBoxItemsPaged2(query).ToList();
                foreach (var item in result)
                {
                    item.DownloadUrl = Url.RouteUrl("ItemDownload2", new { boxId = id, itemId = item.Id });
                }
                return Json(new JsonResponse(true, result));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxId {0} userId {1}", id, userId), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Quizes(long id)
        {
            var userId = User.GetUserId(false);// not really needs it
            try
            {
                var query = new GetBoxItemsPagedQuery(id, userId);
                var result = await ZboxReadService.GetBoxQuizes(query);

                var quizDtos = result as QuizDto[] ?? result.ToArray();
                var remove = quizDtos.Where(w => !w.Publish && w.OwnerId != User.GetUserId(false));
                return Json(new JsonResponse(true, quizDtos.Except(remove)));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxId {0} userId {1}", id, userId), ex);
                return Json(new JsonResponse(false));
            }
        }









        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet]
        [BoxPermission("boxId")]

        public async Task<JsonResult> Members(long boxId)
        {
            var userId = User.GetUserId(false);
            var result = await ZboxReadService.GetBoxMembers(new GetBoxQuery(boxId, userId));
            return Json(new JsonResponse(true, result));
        }




        [HttpPost]
        [ZboxAuthorize]
        public JsonResult UpdateInfo(UpdateBoxInfo model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = User.GetUserId();
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
                ModelState.AddModelError(string.Empty, BoxControllerResources.BoxExists);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("on UpdateBox info model: {0} userid {1}", model, User.GetUserId()), ex);
                return Json(new JsonResponse(false));
            }
        }

        [ZboxAuthorize]
        [HttpGet]
        public JsonResult GetNotification(long boxId)
        {
            var userId = User.GetUserId();

            var result = ZboxReadService.GetUserBoxNotificationSettings(new GetBoxQuery(boxId, userId));
            return Json(new JsonResponse(true, result.ToString("g")));
        }

        [ZboxAuthorize]
        [HttpPost]
        public JsonResult ChangeNotification(long boxId, NotificationSettings notification)
        {
            var userId = User.GetUserId();
            var command = new ChangeNotificationSettingsCommand(boxId, userId, notification);
            ZboxWriteService.ChangeNotificationSettings(command);
            return Json(new JsonResponse(true));

        }


        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
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
        [ZboxAuthorize]
        [Obsolete]
        public JsonResult Delete(long boxUid)
        {
            return Delete2(boxUid);


        }

        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        public JsonResult Delete2(long id)
        {
            var userId = User.GetUserId();
            var command = new UnfollowBoxCommand(id, userId);
            ZboxWriteService.UnFollowBox(command);
            return Json(new JsonResponse(true));
        }


        [NonAction]
        private JsonResult DeleteUserFomBox(long boxId, long userToDeleteId)
        {
            try
            {
                var userId = User.GetUserId();
                var command = new DeleteUserFromBoxCommand(userId, userToDeleteId, boxId);
                ZboxWriteService.DeleteUserFromBox(command);

                //Check
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeleteSubscription user: {0} boxid: {1}", User.GetUserId(), boxId), ex);
                return Json(new JsonResponse(false));
            }
        }

        /// <summary>
        /// Box Setting page
        /// </summary>
        /// <param name="boxId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [ZboxAuthorize]
        public JsonResult RemoveUser(long boxId, long userId)
        {
            return DeleteUserFomBox(boxId, userId);
        }
        #endregion




        #region Tab


        [HttpPost, ZboxAuthorize]
        public JsonResult CreateTab(CreateBoxItemTab model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            try
            {
                var userId = User.GetUserId();
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

        [HttpPost, ZboxAuthorize]
        public JsonResult AddItemToTab(AssignItemToTab model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            model.ItemId = model.ItemId ?? new long[0];
            var userId = User.GetUserId();
            var command = new AssignItemToTabCommand(model.ItemId, model.TabId, model.BoxId, userId, model.NDelete);
            ZboxWriteService.AssignBoxItemToTab(command);
            return Json(new JsonResponse(true));
        }

        [HttpPost, ZboxAuthorize]
        public JsonResult RenameTab(ChangeBoxItemTabName model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = User.GetUserId();
            var command = new ChangeItemTabNameCommand(model.TabId, model.Name, userId, model.BoxId);
            ZboxWriteService.RenameBoxItemTab(command);
            return Json(new JsonResponse(true));
        }
        [HttpPost, ZboxAuthorize]
        public JsonResult DeleteTab(DeleteBoxItemTab model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = User.GetUserId();
            var command = new DeleteItemTabCommand(userId, model.TabId, model.BoxId);
            ZboxWriteService.DeleteBoxItemTab(command);
            return Json(new JsonResponse(true));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
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
        public JsonResult DeleteUpdates(long boxId)
        {
            var userId = User.GetUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            ZboxWriteService.DeleteUpdates(command);
            return Json(new JsonResponse(true));
        }

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]

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

        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]


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


        [HttpGet]
        [OutputCache(CacheProfile = "PartialCache")]
        public ActionResult SocialInvitePartial()
        {
            try
            {
                return PartialView("_Invite");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("_Invite", ex);
                return Json(new JsonResponse(false));
            }
        }





    }
}
