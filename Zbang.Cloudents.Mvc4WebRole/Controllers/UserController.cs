using System.Linq;
using DevTrends.MvcDonutCaching;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversity]
    public class UserController : BaseController
    {
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult IndexPartial()
        {
            return PartialView("Index2");
        }

        //[NonAction]
        //private async Task<UserMinProfile> GetUserProfile(long userId)
        //{
        //    var query = new GetUserMinProfileQuery(userId);
        //    var result = await ZboxReadService.GetUserMinProfile(query);
        //    return result;
        //}

        [DonutOutputCache(VaryByParam = "id", Duration = TimeConsts.Hour)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ProfileStats(long id)
        {
            var query = new GetUserWithFriendQuery(User.GetUserId(), id);
            var model = await ZboxReadService.GetUserProfileWithStatsAsync(query);
            return JsonOk(model);
        }



        [HttpGet]
        public async Task<ActionResult> Friends(long id, int page)
        {
            try
            {
                var friendQuery = new GetUserFriendsQuery(id, page, 40);
                var friendData = await ZboxReadService.GetUserFriendsAsync(friendQuery);

                return JsonOk(friendData);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("GetUserFriends user {0}", User.Identity.Name), ex);
                return JsonError("Problem with get user friends");
            }
        }



        [HttpGet]
        public async Task<ActionResult> Boxes(long id, int page)
        {
            try
            {
                var query = new GetUserWithFriendQuery(User.GetUserId(), id, page, 20);
                var model = await ZboxReadService.GetUserBoxesActivityAsync(query);
                return JsonOk(model);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("User/Boxes user {0}, userRequest {1}", User.Identity.Name, id), ex);
                return JsonError();
            }
        }


        [HttpGet]
        public async Task<ActionResult> Comment(long id, int page)
        {
            var query = new GetUserWithFriendQuery(User.GetUserId(), id, page, 20);
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

        [HttpGet]
        public async Task<ActionResult> Items(long id, int page)
        {
            var query = new GetUserWithFriendQuery(User.GetUserId(), id, page, 20);
            var result = await ZboxReadService.GetUserItemsActivityAsync(query);
            return JsonOk(result);
        }

        [HttpGet]
        public async Task<ActionResult> Quiz(long id, int page)
        {
            var query = new GetUserWithFriendQuery(User.GetUserId(), id, page, 20);
            var result = await ZboxReadService.GetUserQuizActivityAsync(query);
            return JsonOk(result);
        }

        [HttpPost]
        public async Task<ActionResult> GoogleContacts(string token)
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
        [HttpGet]
        public async Task<JsonResult> Notification()
        {
            var userid = User.GetUserId();
            var query = new GetUserDetailsQuery(userid);
            var result = await ZboxReadService.GetUserBoxesNotificationAsync(query);
            return JsonOk(result);
        }


        [HttpGet]
        public async Task<ActionResult> Updates()
        {
            var model = await ZboxReadService.GetUpdatesAsync(new QueryBase(User.GetUserId()));
            return JsonOk(model);
        }
    }
}
