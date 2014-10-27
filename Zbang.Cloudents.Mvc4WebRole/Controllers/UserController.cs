using DevTrends.MvcDonutCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.UserDtos;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.Queries.Boxes;
using Zbang.Zbox.ViewModel.Queries.Library;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]
    [NoUniversity]
    public class UserController : BaseController
    {
        public const int AdminReputation = 1000000;
       
        [NoCache]
        public async Task<ActionResult> Index(long userId, string userName)
        {

            var id = userId;
            ViewBag.Admin = false;

            var model = await GetUserProfile(id);

            if (id != User.GetUserId())
            {
                var userProfile = await GetUserProfile(User.GetUserId());
                if (userProfile.Score > AdminReputation)
                {
                    ViewBag.Admin = true;
                }
            }
            else
            {
                if (model.Score > AdminReputation)
                {
                    ViewBag.Admin = true;
                }
            }
            if (UrlBuilder.NameToQueryString(model.Name) != userName)
            {
                return RedirectToAction("Index", "Error");
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [NonAction]
        private async Task<UserMinProfile> GetUserProfile(long userId)
        {
            var query = new GetUserMinProfileQuery(userId);
            var result = await ZboxReadService.GetUserMinProfile(query);
            return result;
        }

        [DonutOutputCache(VaryByParam = "userId", Duration = TimeConsts.Hour)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> MinProfile(long userId)
        {
            return Json(new JsonResponse(true, await GetUserProfile(userId)));
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Friends(long? userId)
        {
            try
            {
                var query = new GetUserFriendsQuery(User.GetUserId());
                var taskUserData = ZboxReadService.GetUserFriends(query);
                Task<IEnumerable<UserDto>> taskFriendData = Task.FromResult<IEnumerable<UserDto>>(null);
                if (userId.HasValue)
                {
                    var friendQuery = new GetUserFriendsQuery(userId.Value);
                    taskFriendData = ZboxReadService.GetUserFriends(friendQuery);
                }
                await Task.WhenAll(taskUserData, taskFriendData);

                IEnumerable<UserDto> friendFriends = null;
                if (taskFriendData.Result != null)
                {
                    friendFriends = taskFriendData.Result.Where(w => w.Id != User.GetUserId());
                }
                return Json(new JsonResponse(true, new
                {
                    my = taskUserData.Result,
                    user = friendFriends
                }));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("GetUserFriends user {0}", User.Identity.Name), ex);
                return Json(new JsonResponse(false, "Problem with get user friends"));
            }
        }



        [HttpGet, Ajax]
        public async Task<ActionResult> Boxes(long userId)
        {
            try
            {
                var query = new GetUserWithFriendQuery(User.GetUserId(), userId);
                var model = await ZboxReadService.GetUserWithFriendBoxes(query);
                return Json(new JsonResponse(true, model));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("User/Boxes user {0}, userRequest {1}", User.Identity.Name, userId), ex);
                return Json(new JsonResponse(false));
            }
        }

        #region Admin
        [HttpGet, Ajax]
        public async Task<ActionResult> AdminBoxes(long userId)
        {
            try
            {
                var userDetail = FormsAuthenticationService.GetUserData();

                var usermodel = await GetUserProfile(User.GetUserId());
                if (usermodel.Score < AdminReputation)
                {
                    return Json(new JsonResponse(false));
                }
                var universityId = userDetail.UniversityId.Value;
                var query = new GetUserWithFriendQuery(universityId, userId);
                var model = await ZboxReadService.GetUserWithFriendBoxes(query);
                return Json(new JsonResponse(true, model));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("User/Boxes user {0}, userRequest {1}", User.Identity.Name, userId), ex);
                return Json(new JsonResponse(false));
            }
        }
        [HttpGet, Ajax]
        public async Task<ActionResult> AdminFriends()
        {
            var userDetail = FormsAuthenticationService.GetUserData();

            var userModel = await GetUserProfile(User.GetUserId());
            if (userModel.Score < AdminReputation)
            {
                return Json(new JsonResponse(false));
            }

            var universityId = userDetail.UniversityId.Value;
            var query = new GetAdminUsersQuery(universityId);
            var result = await ZboxReadService.GetUniversityUsers(query);
            return Json(new JsonResponse(true, result));
        }
        #endregion


        [HttpGet, Ajax]
        public async Task<ActionResult> OwnedInvites()
        {
            try
            {
                var query = new GetInvitesQuery(User.GetUserId());
                var model = await ZboxReadService.GetUserPersonalInvites(query);


                return Json(new JsonResponse(true, model));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("User/OwnedInvites user {0}", User.Identity.Name), ex);
                return Json(new JsonResponse(false));
            }
        }

        [HttpGet, Ajax]
        public async Task<ActionResult> Activity(long userId)
        {
            var query = new GetUserWithFriendQuery(User.GetUserId(), userId);
            var model = await ZboxReadService.GetUserWithFriendActivity(query);
            return Json(new JsonResponse(true, model));
        }

        [HttpPost, Ajax]
        public async Task<ActionResult> GoogleContacts(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Json(new JsonResponse(false));
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
                    return Json(new JsonResponse(true, result));
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on google contact token" + token, ex);
                return Json(new JsonResponse(false));
            }


        }

        /// <summary>
        /// Used in account settings notification
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Notification()
        {
            var userid = User.GetUserId();
            var query = new GetUserDetailsQuery(userid);
            var result = ZboxReadService.GetUserBoxesNotification(query);
            return Json(new JsonResponse(true, result));
        }


        [HttpGet, Ajax]
        public async Task<ActionResult> Updates()
        {
            var model = await ZboxReadService.GetUpdates(new QueryBase(User.GetUserId()));
            return Json(new JsonResponse(true, model));
        }
    }
}
