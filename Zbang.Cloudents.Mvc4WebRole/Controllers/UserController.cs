using System.Linq;
using DevTrends.MvcDonutCaching;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]

    [NoUniversity]
    public class UserController : BaseController
    {
        [DonutOutputCache(CacheProfile = "FullPage")]
        [NoUniversity]
        [Route("user/{userId:long}/{userName}", Name = "User")]
        [Route("user/{userId:long}/{userName}/{part:regex(^(badges|items|quizzes|feed|members))}", Name = "User2")]
        public ActionResult Index(long userId)
        {
            if (userId == 22886)
            {
                return RedirectToRoute("homePage");
            }
            return View("Empty");
        }

        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");
        }

        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult Badges()
        {
            return PartialView("Badges");
        }
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult Uploads()
        {
            return PartialView("Items");
        }
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult Posts()
        {
            return PartialView("Feed");
        }
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult Quizzes()
        {
            return PartialView("Quizzes");
        }
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult Classmates()
        {
            return PartialView("Members");
        }


        [HttpGet, ActionName("ProfileStats")]
        public async Task<ActionResult> ProfileStatsAsync(long id)
        {
            var query = new GetUserWithFriendQuery(id);
            var model = await ZboxReadService.GetUserProfileWithStatsAsync(query);
            return JsonOk(model);
        }



        [HttpGet, ActionName("Friends")]
        public async Task<ActionResult> FriendsAsync(long id, int page)
        {
            try
            {
                var friendQuery = new GetUserFriendsQuery(id, page, 60);
                var friendData = await ZboxReadService.GetUserFriendsAsync(friendQuery);

                return JsonOk(friendData);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"GetUserFriends userid: {id}", ex);
                return JsonError("Problem with get user friends");
            }
        }



        [HttpGet, ActionName("Boxes")]
        public async Task<ActionResult> BoxesAsync(long id, int page)
        {
            try
            {
                var query = new GetUserWithFriendQuery(id, page, 20);
                var model = await ZboxReadService.GetUserBoxesActivityAsync(query);
                return JsonOk(model);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"User/Boxes user , userRequest {id}", ex);
                return JsonError();
            }
        }


        [HttpGet, ActionName("Comment")]
        public async Task<ActionResult> CommentAsync(long id, int page)
        {
            var query = new GetUserWithFriendQuery(id, page, 20);
            var model = await ZboxReadService.GetUserCommentActivityAsync(query);


            return JsonOk(model.Select(s =>
            new
            {
                s.Content,
                s.CreationTime,
                s.BoxName,
                s.Url
            }));
        }

        [HttpGet, ActionName("Items")]
        public async Task<ActionResult> ItemsAsync(long id, int page)
        {
            var query = new GetUserWithFriendQuery(id, page, 50);
            var result = await ZboxReadService.GetUserItemsActivityAsync(query);
            return JsonOk(result);
        }

        [HttpGet, ActionName("Quiz")]
        public async Task<ActionResult> QuizAsync(long id, int page)
        {
            var query = new GetUserWithFriendQuery(id, page, 20);
            var result = await ZboxReadService.GetUserQuizActivityAsync(query);
            return JsonOk(result);
        }

        [HttpPost, ActionName("GoogleContacts")]
        public async Task<ActionResult> GoogleContactsAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return JsonError();
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = "https://www.google.com/m8/feeds/contacts/default/full?access_token=" + token +
                              "&v=3.0&alt=json&max-results=9999";
                    //somehow without implementing browser this give 403
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                        "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36");
                    var result =
                        await
                            httpClient.GetStringAsync(url);
                    //return Content(result, "text/json");
                    return JsonOk(result);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on google contact token" + token, ex);
                return JsonError();
            }


        }

        /// <summary>
        /// Used in account settings notification
        /// </summary>
        /// <returns></returns>
        [HttpGet, ZboxAuthorize, ActionName("Notification")]
        public async Task<JsonResult> NotificationAsync()
        {
            var userid = User.GetUserId();
            var query = new QueryBaseUserId(userid);
            var result = await ZboxReadService.GetUserBoxesNotificationAsync(query);
            return JsonOk(result);
        }


        [HttpGet, ZboxAuthorize]
        [ActionName("Updates")]
        public async Task<ActionResult> UpdatesAsync()
        {
            var model = await ZboxReadService.GetUpdatesAsync(new QueryBase(User.GetUserId()));
            return JsonOk(model.Select(s => new
            {
                s.AnswerId,
                s.BoxId,
                s.QuestionId
            }));
        }


        [HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult InfoDialog()
        {
            return PartialView();
        }


        [HttpGet, ZboxAuthorize, ActionName("GamificationBoard")]
        public async Task<JsonResult> GamificationBoardAsync()
        {
            var query = new QueryBaseUserId(User.GetUserId());
            var result = await ZboxReadService.GamificationBoardAsync(query);
            var level = GamificationLevels.GetLevel(result.Score);
            result.Level = level.Name;
            result.NextLevel = level.NextLevel;
            result.Number = level.Level;
            return JsonOk(result);
        }



    }
}
