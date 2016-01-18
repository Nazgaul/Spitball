using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
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
        //[DonutOutputCache(VaryByCustom = CustomCacheKeys.Lang,
        //   Duration = TimeConsts.Day, VaryByParam = "boxId",
        //   Location = OutputCacheLocation.Server, Order = 4)]
        [BoxPermission("boxId", Order = 3)]
        public async Task<ActionResult> Index(long boxId, string boxName, string invId)
        {
            var userId = User.GetUserId(false);
            try
            {
                var query = new GetBoxSeoQuery(boxId, userId);
                var model = await ZboxReadService.GetBoxSeoAsync(query);
                if (model == null)
                {
                    throw new BoxDoesntExistException("model is null");
                }
                //var urlBoxName = Server.UrlDecode(Request.Url.Segments[4]);
                if (UrlConsts.NameToQueryString(model.Name) != boxName)
                {
                    throw new BoxDoesntExistException(Request.Url.AbsoluteUri);
                }
                if (model.BoxType == BoxType.Box)
                {
                    ViewBag.title = string.Format("{0} | {1}", model.Name, BaseControllerResources.Cloudents);
                    return View("Empty");
                }
                BaseControllerResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
                ViewBag.title = string.Format("{0} | {1} | {2} | {3}", model.Name, model.DepartmentName,
                    model.UniversityName, BaseControllerResources.Cloudents);
                ViewBag.metaDescription = Regex.Replace(string.Format(
                    BaseControllerResources.MetaDescription, model.Name,
                    string.IsNullOrWhiteSpace(model.CourseId) ? string.Empty : string.Format(", #{0}", model.CourseId),
                        string.Empty
                         ),
                    @"\s+", " ");
                return View("Empty");
            }
            catch (BoxAccessDeniedException)
            {
                return Request.Url == null ? RedirectToAction("MembersOnly", "Error")
                    : RedirectToAction("MembersOnly", "Error",
                    new { returnUrl = Request.Url.AbsolutePath, invId });
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


        //[PreserveQueryString]
        public async Task<ActionResult> ShortUrl(string box62Id)
        {
            var base62 = new Base62(box62Id);
            var userId = User.GetUserId(false);
            var query = new GetBoxSeoQuery(base62.Value, userId);
            try
            {
                var model = await ZboxReadService.GetBoxSeoAsync(query);
                return RedirectPermanent(model.Url);
            }
            catch (BoxDoesntExistException ex)
            {
                TraceLog.WriteError("base 62: " + box62Id + " id:" + base62.Value, ex);
                return RedirectToAction("Index", "Dashboard");
            }


        }
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult IndexPartial()
        {
            return PartialView("Index2");
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult FeedPartial()
        {
            return PartialView("_Feed");
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult ItemsPartial()
        {
            return PartialView("_Items");
        }
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult QuizPartial()
        {
            return PartialView("_Quizzes2");
        }
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult MembersPartial()
        {
            return PartialView("_Members2");
        }



        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<ActionResult> Data(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var result = await ZboxReadService.GetBox2Async(query);
                result.UserType = ViewBag.UserType;
                result.ShortUrl = UrlConsts.BuildShortBoxUrl(new Base62(id).ToString());

                if (IsCrawler())
                {
                    return JsonOk(new
                    {
                        result.BoxType,
                        result.CourseId,
                        result.Date,
                        result.Feeds,
                        result.Items,
                        result.Members,
                        result.Name,
                        result.OwnerId,
                        result.PrivacySetting,
                        result.Quizes,
                        result.ShortUrl,
                        result.UserType
                    });
                }

                return JsonOk(result);
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
                TraceLog.WriteError(string.Format("Box Index id {0}", id), ex);
                return JsonError();
            }
        }


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Recommended(long id)
        {
            var query = new GetBoxSideBarQuery(id, User.GetUserId(false));
            var result = await ZboxReadService.GetBoxRecommendedCoursesAsync(query);
            return JsonOk(result);
        }
        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> LeaderBoard(long id)
        {
            var query = new GetLeaderBoardQuery(id);
            var result = await ZboxReadService.GetBoxLeaderBoardAsync(query);
            if (IsCrawler())
            {
                return JsonOk(result.Select(s => new
                {
                    s.Id,
                    s.Image,
                    s.Score,
                    s.Url,
                }));
            }
            return JsonOk(result);
        }

        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Tabs(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var result = await ZboxReadService.GetBoxTabsAsync(query);
                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Tabs id {0}", id), ex);
                return JsonError();
            }
        }


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Items(long id, int page, Guid? tabId = null)
        {
            try
            {
                var query = new GetBoxItemsPagedQuery(id, tabId, page, 25);
                var result = await ZboxReadService.GetBoxItemsPagedAsync(query);
                var itemDtos = result as IList<ItemDto> ?? result.ToList();
                foreach (var item in itemDtos)
                {
                    item.DownloadUrl = Url.RouteUrl("ItemDownload2", new { boxId = id, itemId = item.Id });
                }
                //return JsonOk(result);
                return JsonOk(itemDtos.Select(s => new
                {
                    //s.CommentsCount,
                    //s.Date,
                    //s.Description,
                    //s.DownloadUrl,
                    s.Id,
                    s.Name,
                    NumOfViews = s.NumOfViews + s.NumOfDownloads,
                    //s.Owner,
                    s.OwnerId,
                    s.Likes,
                    //s.Sponsored,
                    //s.TabId,
                    //s.Thumbnail,
                    s.Url,
                    //s.UserUrl,
                    s.Type,
                    s.Source

                }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxId {0}", id), ex);
                return JsonError();
            }
        }

        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id")]
        public async Task<JsonResult> Quizes(long id)
        {
            try
            {
                var query = new GetBoxQuizesPagedQuery(id);
                var result = await ZboxReadService.GetBoxQuizesAsync(query);
                var userid = User.GetUserId(false);
                var quizDtos = result as QuizDto[] ?? result.ToArray();
                var remove = quizDtos.Where(w => !w.Publish && w.OwnerId != userid);
                return JsonOk(quizDtos.Except(remove));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Box Items BoxId {0} ", id), ex);
                return JsonError();
            }
        }









        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet]
        [BoxPermission("boxId")]

        public async Task<JsonResult> Members(long boxId)
        {
            var result = await ZboxReadService.GetBoxMembersAsync(new GetBoxQuery(boxId));
            return JsonOk(result);
        }




        [HttpPost]
        [ZboxAuthorize]
        public JsonResult UpdateInfo(UpdateBoxInfo model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var userId = User.GetUserId();
            try
            {
                var commandBoxName = new ChangeBoxInfoCommand(model.Id, userId, model.Name,
                    model.Professor, model.CourseCode, model.BoxPrivacy, model.Notification);
                ZboxWriteService.ChangeBoxInfo(commandBoxName);
                return JsonOk(new { queryString = UrlBuilder.NameToQueryString(model.Name) });
            }
            catch (UnauthorizedAccessException)
            {
                return JsonError("You don't have permission");
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(string.Empty, BoxControllerResources.BoxExists);
                return JsonError(GetErrorFromModelState());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("on UpdateBox info model: {0} userid {1}", model, User.GetUserId()), ex);
                return JsonError();
            }
        }

        [ZboxAuthorize]
        [HttpGet]
        public async Task<JsonResult> GetNotification(long boxId)
        {
            var userId = User.GetUserId();

            var result = await ZboxReadService.GetUserBoxNotificationSettingsAsync(new GetBoxQuery(boxId), userId);
            return JsonOk(result);
        }

        [ZboxAuthorize]
        [HttpPost]
        public JsonResult ChangeNotification(long boxId, NotificationSettings notification)
        {
            var userId = User.GetUserId();
            var command = new ChangeNotificationSettingsCommand(boxId, userId, notification);
            ZboxWriteService.ChangeNotificationSettings(command);
            return JsonOk();

        }




        #region DeleteBox


        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        public async Task<JsonResult> Delete(long id)
        {
            var userId = User.GetUserId();
            var command = new UnFollowBoxCommand(id, userId, false);
            await ZboxWriteService.UnFollowBoxAsync(command);
            return JsonOk();
        }


        //[NonAction]
        //private JsonResult DeleteUserFomBox(long boxId, long userToDeleteId)
        //{
        //    try
        //    {
        //        var userId = User.GetUserId();
        //        var command = new DeleteUserFromBoxCommand(userId, userToDeleteId, boxId);
        //        ZboxWriteService.DeleteUserFromBox(command);

        //        //Check
        //        return JsonOk();
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("DeleteSubscription user: {0} boxid: {1}", User.GetUserId(), boxId), ex);
        //        return JsonError();
        //    }
        //}


        //[HttpPost]
        //[ZboxAuthorize]
        //public JsonResult RemoveUser(long boxId, long userId)
        //{
        //    return DeleteUserFomBox(boxId, userId);
        //}
        #endregion




        #region Tab


        [HttpPost, ZboxAuthorize]
        public async Task<JsonResult> CreateTab(CreateBoxItemTab model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                var userId = User.GetUserId();
                var guid = Guid.NewGuid();
                var command = new CreateItemTabCommand(guid, model.Name, model.BoxId, userId);
                await ZboxWriteService.CreateBoxItemTabAsync(command);
                var result = new TabDto { Id = guid, Name = model.Name };
                return JsonOk(result);
            }
            catch (ArgumentException ex)
            {
                return JsonError(ex.Message);
            }

        }

        [HttpPost, ZboxAuthorize]
        public async Task<JsonResult> AddItemToTab(AssignItemToTab model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var userId = User.GetUserId();
            var command = new AssignItemToTabCommand(model.ItemId, model.TabId, model.BoxId, userId);
            await ZboxWriteService.AssignBoxItemToTabAsync(command);
            return JsonOk();
        }

        [HttpPost, ZboxAuthorize]
        public JsonResult RenameTab(ChangeBoxItemTabName model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var userId = User.GetUserId();
            var command = new ChangeItemTabNameCommand(model.TabId, model.Name, userId, model.BoxId);
            ZboxWriteService.RenameBoxItemTab(command);
            return JsonOk();
        }
        [HttpPost, ZboxAuthorize]
        public JsonResult DeleteTab(DeleteBoxItemTab model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var userId = User.GetUserId();
            var command = new DeleteItemTabCommand(userId, model.TabId.Value, model.BoxId);
            ZboxWriteService.DeleteBoxItemTab(command);
            return JsonOk();
        }


        #endregion

        [ZboxAuthorize]
        [HttpPost]
        public JsonResult DeleteUpdates(long boxId)
        {
            var userId = User.GetUserId();
            try
            {
                
                var command = new DeleteUpdatesCommand(userId, boxId);
                ZboxWriteService.DeleteUpdates(command);
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("delete update boxid " + boxId + " userid" + userId, ex);
                return JsonError();
            }
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
        //        return JsonError();
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
        //        return JsonError();
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
        //        return JsonError();
        //    }
        //}





    }
}
