﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models;
using Zbang.Cloudents.Mvc4WebRole.Models.Tabs;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure;
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
        public ActionResult RedirectToNewRoute(long boxId, string boxName, string invId)
        {
            return RedirectToRoutePermanent("CourseBox", new RouteValueDictionary
            {
                ["boxId"] = boxId,
                ["boxName"] = boxName,
                ["invId"] = invId,
                ["universityName"] = "my"
                //["part"] = "feed"
            });
        }

        [Route("box/my/{boxId:long}/{boxName}/{part:regex(^(feed|items|quizzes|members|flashcards))}", Name = "PrivateBoxWithSub")]
        public ActionResult RedirectToNewRoute2(long boxId, string boxName, string invId, string part)
        {
            return RedirectToRoutePermanent("CourseBox", new RouteValueDictionary
            {
                ["boxId"] = boxId,
                ["boxName"] = boxName,
                ["invId"] = invId,
                ["universityName"] = "my",
                ["part"] = part
                //["part"] = "feed"
            });
        }

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
        [Route("course/{universityName}/{boxId:long}/{boxName}/{part:regex(^(feed|items|quizzes|members|flashcards))}", Name = "CourseBoxWithSub")]
        public async Task<ActionResult> IndexAsync(long boxId,string universityName, string boxName, string invId, string part)
        {
            try
            {
                var query = new GetBoxIdQuery(boxId);
                var model = await ZboxReadService.GetBoxSeoAsync(query).ConfigureAwait(false);
                if (model == null)
                {
                    throw new BoxDoesntExistException("model is null");
                }
                if (UrlConst.NameToQueryString(model.Name) != boxName)
                {
                    return Redirect(Url.RouteUrlCache("CourseBoxWithSub", new RouteValueDictionary
                    {
                        ["boxId"] = boxId,
                        ["universityName"] = universityName,
                        ["boxName"] = UrlConst.NameToQueryString(model.Name),
                        ["part"] = part
                    }));
                    //return Redirect(model.Url);
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
                    ViewBag.title =
                        $"{SeoBaseUniversityResources.BoxTitleItems} - {model.Name} - {model.DepartmentName} | {SeoBaseUniversityResources.Cloudents}";
                }
                if (part == "quizzes")
                {
                    ViewBag.title =
                        $"{SeoBaseUniversityResources.BoxTitleQuizzes} - {model.Name} - {model.DepartmentName} | {SeoBaseUniversityResources.Cloudents}";
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
                return RedirectToAction("NotFound", "Error");
            }
            catch (Exception ex)
            {
                var userId = User.GetUserId(false);
                TraceLog.WriteError($"Box Index boxId {boxId} userId {userId}", ex);
                return RedirectToAction("Index", "Error");
            }
        }

        [Route(UrlConst.ShortBox, Name = "shortBox"), NoUrlLowercase]
        public async Task<ActionResult> ShortUrlAsync(string box62Id)
        {
            var base62 = new Base62(box62Id);
            var query = new GetBoxIdQuery(base62.Value);
            try
            {
                var model = await ZboxReadService.GetBoxSeoAsync(query).ConfigureAwait(false);
                // [Route("course/{universityName}/{boxId:long}/{boxName}/{part:regex(^(feed|items|quizzes|members|flashcards))}", Name = "CourseBoxWithSub")]
                var url = Url.RouteUrlCache("CourseBoxWithSub", new RouteValueDictionary
                {
                    ["boxId"] = base62.Value,
                    ["universityName"] = UrlConst.NameToQueryString(model.UniversityName ?? "my"),
                    ["boxName"] = UrlConst.NameToQueryString(model.Name),
                    ["part"] = "feed"
                });
                if (Request.IsAjaxRequest())
                {
                    return JsonOk(url);
                }
                return RedirectPermanent(url);
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

        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public PartialViewResult LeaderboardPartial()
        {
            return PartialView("LeaderboardPartial");
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
                        result.UserType
                    });
                }

                //TODO: check what we can remove
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
                    result.OwnerName,
                    result.PrivacySetting,
                    result.ProfessorName,
                    result.Quizes,
                    result.UserType

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
                TraceLog.WriteError($"Box Index id {id}", ex);
                return JsonError();
            }
        }

        //[HttpGet]
        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //[BoxPermission("id"), ActionName("Recommended")]
        //[AsyncTimeout(TimeConst.Second * 3 * 1000)]
        //public async Task<JsonResult> RecommendedAsync(long id, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        using (var token = CreateCancellationToken(cancellationToken))
        //        {
        //            var userId = User.GetUserId(false);
        //            var query = new GetBoxSideBarQuery(id, userId);
        //            var result = await ZboxReadService.GetBoxRecommendedCoursesAsync(query, token.Token);
        //            return JsonOk(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError($"Recommended {id}", ex);
        //        return JsonOk();
        //    }
        //}

        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [BoxPermission("id"), ActionName("LeaderBoard")]
        public async Task<JsonResult> LeaderBoardAsync(long id)
        {
            //myself = User.Identity.IsAuthenticated && myself;
            var userId = User.GetUserId(false);
            var query = new GetBoxLeaderboardQuery(id, userId);
            var model = await ZboxReadService.GetBoxLeaderBoardAsync(query);
            var leaderBoardDtos = model as IList<LeaderBoardDto> ?? model.ToList();
            var rank = leaderBoardDtos.FirstOrDefault(w => w.Id == userId)?.Location;
            if (rank.HasValue && rank > 10)
            {
                leaderBoardDtos = leaderBoardDtos.Where(w => w.Id != userId).Select(s =>
                {
                    s.LevelName = GamificationLevels.GetLevel(s.Score).Name;
                    return s;
                }).ToList();
            }

            if (IsCrawler())
            {
                return JsonOk(new
                {
                    rank,
                    model = leaderBoardDtos.Select(s => new
                    {
                        s.Id,
                        s.Image,
                        s.Score,
                        s.LevelName,
                        s.Location

                    })
                });
            }
            return JsonOk(new { rank, model = leaderBoardDtos });
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
                return JsonOk(itemDtos.Select(s => new
                {
                    s.Id,
                    s.Name,
                    NumOfViews = s.NumOfViews + s.NumOfDownloads,
                    s.OwnerId,
                    s.Likes,
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
                var result = await ZboxReadService.GetBoxQuizzesAsync(query);
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
