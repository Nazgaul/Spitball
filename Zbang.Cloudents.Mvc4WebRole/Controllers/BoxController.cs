using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.Mvc4WebRole.Models.Tabs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
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
        [Route("box/my/{boxId:long}/{boxName}", Name = "PrivateBox")]
        [Route("course/{universityName}/{boxId:long}/{boxName}", Name = "CourseBox")]
        public ActionResult RedirectToFeed(long boxId, string boxName, string invId, string universityName)
        {
            if (string.IsNullOrEmpty(universityName))
            {
                return RedirectToRoutePermanent("PrivateBoxWithSub", new RouteValueDictionary
                {
                    ["boxId"] = boxId,
                    ["boxName"] = boxName,
                    ["invId"] = invId,
                    ["part"] = "feed"
                });
            }
            return RedirectToRoutePermanent("CourseBoxWithSub", new RouteValueDictionary
            {
                ["universityName"] = universityName,
                ["boxId"] = boxId,
                ["boxName"] = boxName,
                ["invId"] = invId,
                ["part"] = "feed"
            });
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "BoxPage")]
        [BoxPermission("boxId", Order = 3), ActionName("Index")]
        [Route("box/my/{boxId:long}/{boxName}/{part:regex(^(feed|items|quizzes|members|flashcards))}", Name = "PrivateBoxWithSub")]
        [Route("course/{universityName}/{boxId:long}/{boxName}/{part:regex(^(feed|items|quizzes|members|flashcards))}", Name = "CourseBoxWithSub")]
        public async Task<ActionResult> IndexAsync(long boxId, string boxName, string invId, string part)
        {

            try
            {
                var query = new GetBoxSeoQuery(boxId);
                var model = await ZboxReadService.GetBoxSeoAsync(query);
                if (model == null)
                {
                    throw new BoxDoesntExistException("model is null");
                }
                if (UrlConst.NameToQueryString(model.Name) != boxName)
                {
                    return Redirect(model.Url);
                }
                SeoBaseUniversityResources.Culture = Languages.GetCultureBaseOnCountry(model.Country);
                if (model.BoxType == BoxType.Box)
                {
                    ViewBag.title = $"{model.Name} | {SeoBaseUniversityResources.Cloudents}";
                    return View("Empty");
                }


                ViewBag.metaDescription = string.Format(SeoBaseUniversityResources.BoxMetaDescription, model.Name);

                if (part == "feed" || part == "members")
                {
                    ViewBag.title = $"{model.Name} - {model.UniversityName} | {SeoBaseUniversityResources.Cloudents}";
                }
                if (part == "items")
                {
                    ViewBag.title = string.Format("{3} - {0} - {1} | {2}", model.Name, model.DepartmentName,
                        SeoBaseUniversityResources.Cloudents, SeoBaseUniversityResources.BoxTitleItems);
                }
                if (part == "quizzes")
                {
                    ViewBag.title = string.Format("{3} - {0} - {1} | {2}", model.Name, model.DepartmentName,
                        SeoBaseUniversityResources.Cloudents, SeoBaseUniversityResources.BoxTitleQuizzes);
                }
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
                var userId = User.GetUserId(false);
                TraceLog.WriteError($"Box Index boxId {boxId} userid {userId}", ex);
                return RedirectToAction("Index", "Error");
            }
        }


        [Route(UrlConst.ShortBox, Name = "shortBox"), NoUrlLowercase]
        public async Task<ActionResult> ShortUrlAsync(string box62Id)
        {
            var base62 = new Base62(box62Id);
            var query = new GetBoxSeoQuery(base62.Value);
            try
            {
                var model = await ZboxReadService.GetBoxSeoAsync(query);
                return RedirectPermanent(model.Url);
            }
            catch (BoxDoesntExistException ex)
            {
                TraceLog.WriteError($"base 62: {box62Id} id: {base62.Value}", ex);
                return RedirectToAction("NotFound", "Error");
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

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult FlashcardsPartial()
        {
            return PartialView("_Flashcards");
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult BoxSettings()
        {
            return PartialView("_Settings");
        }


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id"), ActionName("Data")]
        public async Task<ActionResult> DataAsync(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var result = await ZboxReadService.GetBox2Async(query);
                result.UserType = ViewBag.UserType;
                result.ShortUrl = UrlConst.BuildShortBoxUrl(new Base62(id).ToString());

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
                TraceLog.WriteError($"Box Index id {id}", ex);
                return JsonError();
            }
        }


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id"), ActionName("Recommended")]
        [AsyncTimeout(TimeConst.Second * 3 * 1000)]
        public async Task<JsonResult> RecommendedAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                using (var token = CreateCancellationToken(cancellationToken))
                {
                    var userId = User.GetUserId(false);
                    var query = new GetBoxSideBarQuery(id, userId);
                    var result = await ZboxReadService.GetBoxRecommendedCoursesAsync(query, token.Token);
                    return JsonOk(result);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Recommended {id}", ex);
                return JsonOk();
            }
        }
        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id"), ActionName("LeaderBoard")]
        public async Task<JsonResult> LeaderBoardAsync(long id)
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
        [BoxPermission("id"), ActionName("Tabs")]
        public async Task<JsonResult> TabsAsync(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var result = await ZboxReadService.GetBoxTabsAsync(query);
                return JsonOk(result);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Box Tabs id {id}", ex);
                return JsonError();
            }
        }


        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id"), ActionName("Items")]
        public async Task<JsonResult> ItemsAsync(long id, int page, Guid? tabId = null)
        {
            try
            {
                var query = new GetBoxItemsPagedQuery(id, tabId, page, 80);
                var result = await ZboxReadService.GetBoxItemsPagedAsync(query);
                var itemDtos = result as IList<ItemDto> ?? result.ToList();
                foreach (var item in itemDtos)
                {
                    item.DownloadUrl = Url.RouteUrl("ItemDownload2", new { boxId = id, itemId = item.Id });
                }
                return JsonOk(itemDtos.Select(s => new
                {
                    s.Id,
                    s.Name,
                    NumOfViews = s.NumOfViews + s.NumOfDownloads,
                    s.OwnerId,
                    s.Likes,
                    s.Url,
                    s.Type,
                    s.Source

                }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Box Items BoxId {id}", ex);
                return JsonError();
            }
        }

        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id"), ActionName("Quizes")]
        public async Task<JsonResult> QuizzesAsync(long id)
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
                TraceLog.WriteError($"Box Quiz BoxId {id} ", ex);
                return JsonError();
            }
        }

        [HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id"), ActionName("FlashCards")]
        public async Task<JsonResult> FlashcardsAsync(long id)
        {
            try
            {
                var query = new GetFlashCardsQuery(id);
                var result = await ZboxReadService.GetBoxFlashcardsAsync(query);
                var userid = User.GetUserId(false);
                var data = result.Where(w => w.Publish || w.OwnerId == userid).Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.NumOfViews,
                    s.Publish,
                    s.OwnerId,
                    s.Likes
                });

                return JsonOk(data);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"Box Flashcards BoxId {id} ", ex);
                return JsonError();
            }
        }









        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet]
        [BoxPermission("boxId"), ActionName("Members")]

        public async Task<JsonResult> MembersAsync(long boxId)
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
                TraceLog.WriteError($"on UpdateBox info model: {model} userid {User.GetUserId()}", ex);
                return JsonError();
            }
        }

        [ZboxAuthorize]
        [HttpGet, ActionName("GetNotification")]
        public async Task<JsonResult> GetNotificationAsync(long boxId)
        {
            var userId = User.GetUserId();

            var result = await ZboxReadService.GetUserBoxNotificationSettingsAsync(new GetBoxQuery(boxId), userId);
            return JsonOk(result);
        }

        [ZboxAuthorize]
        [HttpPost]
        public JsonResult ChangeNotification(long boxId, NotificationSetting notification)
        {
            var userId = User.GetUserId();
            var command = new ChangeNotificationSettingsCommand(boxId, userId, notification);
            ZboxWriteService.ChangeNotificationSettings(command);
            return JsonOk();

        }




        #region DeleteBox


        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie, ActionName("Delete")]
        public async Task<JsonResult> DeleteAsync(long id)
        {
            var userId = User.GetUserId();
            var command = new UnFollowBoxCommand(id, userId, false);
            await ZboxWriteService.UnFollowBoxAsync(command);
            return JsonOk();
        }



        #endregion




        #region Tab


        [HttpPost, ZboxAuthorize, ActionName("CreateTab")]
        public async Task<JsonResult> CreateTabAsync(CreateBoxItemTab model)
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

        [HttpPost, ZboxAuthorize, ActionName("AddItemToTab")]
        public async Task<JsonResult> AddItemToTabAsync(AssignItemToTab model)
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
            try
            {
                var userId = User.GetUserId();
                var command = new ChangeItemTabNameCommand(model.TabId, model.Name, userId, model.BoxId);
                ZboxWriteService.RenameBoxItemTab(command);
                return JsonOk();
            }
            catch (ArgumentException ex)
            {
                return JsonError(ex.Message);
            }
        }
        [HttpPost, ZboxAuthorize]
        public JsonResult DeleteTab(DeleteBoxItemTab model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            var userId = User.GetUserId();
            Debug.Assert(model.TabId != null, "model.TabId != null");
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

        [ZboxAuthorize, HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult LikesDialog()
        {
            return PartialView();
        }

    }
}
