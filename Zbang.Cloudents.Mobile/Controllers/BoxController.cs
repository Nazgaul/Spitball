using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mobile.Controllers.Resources;
using Zbang.Cloudents.Mobile.Extensions;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mobile.Helpers;
using Zbang.Cloudents.Mobile.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mobile.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxHandleError(ExceptionType = typeof(UnauthorizedAccessException), View = "Error")]
    [ZboxHandleError(ExceptionType = typeof(BoxAccessDeniedException), View = "Error")]
    [NoUniversity]
    public class BoxController : BaseController
    {
        

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
                return JsonOk(new
                {
                    result.Name,
                    result.BoxType,
                    result.UserType,
                    result.ProfessorName,
                    result.CourseId
                });
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
                return JsonError();
            }
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


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Items(long id, int page)
        {
            try
            {
                var query = new GetBoxItemsPagedQuery(id, page, 10);
                var result = await ZboxReadService.GetBoxItemsPagedAsync(query);
                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxId {0} page {1}", id, page), ex);
                return JsonError();
            }
        }

        //[HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        //[BoxPermission("id")]
        //public async Task<JsonResult> Quizes(long id)
        //{
        //    var userId = User.GetUserId(false);// not really needs it
        //    try
        //    {
        //        var query = new GetBoxItemsPagedQuery(id, userId);
        //        var result = await ZboxReadService.GetBoxQuizes(query);

        //        var quizDtos = result as QuizDto[] ?? result.ToArray();
        //        var remove = quizDtos.Where(w => !w.Publish && w.OwnerId != User.GetUserId(false));
        //        return Json(new JsonResponse(true, quizDtos.Except(remove)));
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("Box Items BoxId {0} userId {1}", id, userId), ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}









        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //[HttpGet]
        //[BoxPermission("boxId")]

        //public async Task<JsonResult> Members(long boxId)
        //{
        //    var userId = User.GetUserId(false);
        //    var result = await ZboxReadService.GetBoxMembers(new GetBoxQuery(boxId, userId));
        //    return Json(new JsonResponse(true, result));
        //}


        //[ZboxAuthorize]
        //public ActionResult Settings(long boxUid)
        //{

        //    var userId = User.GetUserId();

        //    var query = new GetBoxQuery(boxUid, userId);
        //    var result = ZboxReadService.GetBoxSetting(query);

        //    var model = new BoxSetting
        //      {
        //          Name = result.Name,
        //          Notification = result.NotificationSetting,
        //          Privacy = result.PrivacySetting,
        //          UserType = result.UserType
        //      };
        //    return View(model);
        //}



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

        //[ZboxAuthorize]
        //[HttpGet]
        //public JsonResult GetNotification(long boxId)
        //{
        //    var userId = User.GetUserId();

        //    var result = ZboxReadService.GetUserBoxNotificationSettings(new GetBoxQuery(boxId, userId));
        //    return Json(new JsonResponse(true, result.ToString("g")));
        //}

        //[ZboxAuthorize]
        //[HttpPost]
        //public JsonResult ChangeNotification(long boxId, NotificationSettings notification)
        //{
        //    var userId = User.GetUserId();
        //    var command = new ChangeNotificationSettingsCommand(boxId, userId, notification);
        //    ZboxWriteService.ChangeNotificationSettings(command);
        //    return Json(new JsonResponse(true));

        //}


        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult SettingsPartial()
        //{
        //    try
        //    {
        //        return PartialView("_BoxSettings");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_BoxSettings", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}

        #region DeleteBox

        [HttpPost]
        [ZboxAuthorize]
        public JsonResult Delete2(long id)
        {
            var userId = User.GetUserId();
            var command = new UnFollowBoxCommand(id, userId, false);
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




        //#region Tab


        //[HttpPost, ZboxAuthorize]
        //public JsonResult CreateTab(CreateBoxItemTab model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, GetModelStateErrors()));
        //    }
        //    try
        //    {
        //        var userId = User.GetUserId();
        //        var guid = Guid.NewGuid();
        //        var command = new CreateItemTabCommand(guid, model.Name, model.BoxId, userId);
        //        ZboxWriteService.CreateBoxItemTab(command);
        //        var result = new TabDto { Id = guid, Name = model.Name };
        //        return Json(new JsonResponse(true, result));
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return Json(new JsonResponse(false, ex.Message));
        //    }

        //}

        //[HttpPost, ZboxAuthorize]
        //public JsonResult AddItemToTab(AssignItemToTab model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, GetModelStateErrors()));
        //    }
        //    model.ItemId = model.ItemId ?? new long[0];
        //    var userId = User.GetUserId();
        //    var command = new AssignItemToTabCommand(model.ItemId, model.TabId, model.BoxId, userId, model.NDelete);
        //    ZboxWriteService.AssignBoxItemToTab(command);
        //    return Json(new JsonResponse(true));
        //}

        //[HttpPost, ZboxAuthorize]
        //public JsonResult RenameTab(ChangeBoxItemTabName model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, GetModelStateErrors()));
        //    }
        //    var userId = User.GetUserId();
        //    var command = new ChangeItemTabNameCommand(model.TabId, model.Name, userId, model.BoxId);
        //    ZboxWriteService.RenameBoxItemTab(command);
        //    return Json(new JsonResponse(true));
        //}
        //[HttpPost, ZboxAuthorize]
        //public JsonResult DeleteTab(DeleteBoxItemTab model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json(new JsonResponse(false, GetModelStateErrors()));
        //    }
        //    var userId = User.GetUserId();
        //    var command = new DeleteItemTabCommand(userId, model.TabId, model.BoxId);
        //    ZboxWriteService.DeleteBoxItemTab(command);
        //    return Json(new JsonResponse(true));
        //}

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult CreateTabPartial()
        //{
        //    try
        //    {
        //        return PartialView("_CreateTab");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_CreateTab ", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}
        //#endregion

        [ZboxAuthorize]
        [HttpPost]
        public JsonResult DeleteUpdates(long boxId)
        {
            var userId = User.GetUserId();
            var command = new DeleteUpdatesCommand(userId, boxId);
            ZboxWriteService.DeleteUpdates(command);
            return Json(new JsonResponse(true));
        }

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]

        //public ActionResult UploadPartial()
        //{
        //    try
        //    {
        //        return PartialView("_UploadDialog");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_UploadDialog ", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}

        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]


        //public ActionResult UploadLinkPartial()
        //{
        //    try
        //    {
        //        return PartialView("_UploadAddLink");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_UploadAddLink", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}


        //[HttpGet]
        //[OutputCache(CacheProfile = "PartialCache")]
        //public ActionResult SocialInvitePartial()
        //{
        //    try
        //    {
        //        return PartialView("_Invite");
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("_Invite", ex);
        //        return Json(new JsonResponse(false));
        //    }
        //}





    }
}
