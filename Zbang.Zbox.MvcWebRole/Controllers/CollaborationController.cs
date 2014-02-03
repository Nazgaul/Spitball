using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.MvcWebRole.Helpers;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.MvcWebRole.Controllers
{
    public class CollaborationController : Controller
    {
        private IZboxService m_ZboxService;
        public CollaborationController(IZboxService zboxService)
        {
            m_ZboxService = zboxService;
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteUserFromBox(string userToDeleteEmail, long boxId)
        {
            try
            {
                DeleteUserFromBoxCommand command = new DeleteUserFromBoxCommand(ExtractUserID.GetUserEmailId(), userToDeleteEmail, boxId);
                DeleteUserFromBoxCommandResult result = m_ZboxService.DeleteUserFromBox(command);

                //Check
                return this.Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                //Check
                TraceLog.WriteInfo(string.Format("DeleteUserFromBox user {0} userToDeleteEmail {1} boxId {2}", this.User.Identity.Name, userToDeleteEmail, boxId));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with delete an invite"));
            }
        }

        [Authorize]
        public JsonResult DeleteOwnedSubscription(string boxid)
        {
            long boxId = ShortCodesCache.ShortCodeToLong(boxid);
            MembershipUser membershipUser = Membership.GetUser(this.User.Identity.Name);
            return DeleteUserFromBox(membershipUser.Email, boxId);
        }

        [Authorize]
        [HttpPost]
        public JsonResult ChangeBoxUserRights(string userEmail, long boxId, int NewPermission)
        {
            try
            {
                UserPermissionSettings setting = (UserPermissionSettings)NewPermission;

                ChangeBoxUserRightsCommand command = new ChangeBoxUserRightsCommand(boxId, userEmail, setting, ExtractUserID.GetUserEmailId());
                ChangeBoxUserRightsCommandResult result = m_ZboxService.ChangeBoxUserRights(command);

                JsonResponse retval = new JsonResponse(true);
                if (userEmail.ToLower() == ExtractUserID.GetUserEmailId())
                    //User change his own permissions
                    retval.Payload = NewPermission;

                return this.Json(retval);
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("ChangeBoxUserRights user {0} userEmail {1} box id {2} new permission {3}", this.User.Identity.Name, userEmail, boxId, NewPermission));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with change subscription rights"));
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetUserFriends()
        {
            try
            {
                IEnumerable<FriendDto> result = GetUserCurrentFriends();

                return this.Json(new JsonResponse(true, result), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                TraceLog.WriteInfo(string.Format("GetUserFriends user {0}", this.User.Identity.Name));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with get user friends"));
            }
        }

        private IEnumerable<FriendDto> GetUserCurrentFriends()
        {
            GetUserFriendsQuery query = new GetUserFriendsQuery(ExtractUserID.GetUserEmailId());

            IEnumerable<FriendDto> result = m_ZboxService.GetUserFriends(query);
            return result;
        }

        [Authorize]
        public JsonResult RequestSubscription(long boxId)
        {
            try
            {
                MembershipUser membershipUser = Membership.GetUser(this.User.Identity.Name);

                //Construct Box Uri        
                string approvalUri = string.Format("{0}://{1}/Collaboration/ApproveSubscriptionInvitation?boxId={2}&userId={3}&userAskedForInvitationId={4}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, boxId, "<BoxOwnerId>", ExtractUserID.GetUserEmailId());

                RequestSubscriptionCommand command = new RequestSubscriptionCommand(ExtractUserID.GetUserEmailId(), boxId, approvalUri);
                RequestSubscriptionCommandResult result = m_ZboxService.RequestSubscription(command);

                //Check
                return this.Json(new JsonResponse(true), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("request subscription user {0} box id {1}", this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with request subscription"), JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ShareBox(long boxId, IList<string> to, string personalNote)
        {
          
            try
            {
                ShareBoxCommand command = new ShareBoxCommand(boxId, ExtractUserID.GetUserEmailId(), to, personalNote, this.User.Identity.Name);
                ShareBoxCommandResult result = m_ZboxService.ShareBox(command);

                return this.Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("ShareBox user {0} box id {1} to {2} personal note {3}", this.User.Identity.Name, boxId, string.Join(",", to.ToArray()), personalNote));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with share box"), JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetBoxSubscribersAndInvitations(long boxId)
        {
            try
            {

                GetBoxUsersQuery UsersQuery = new GetBoxUsersQuery(boxId, ExtractUserID.GetUserEmailId());
                IEnumerable<UserDto> boxUsers = m_ZboxService.GetBoxUsers(UsersQuery);

                var boxSubscribers = boxUsers.Where(w => w.UserType == UserType.subscribe);
                var boxInvitations = boxUsers.Where(w => w.UserType == UserType.invite);

                var response = new { Subscribers = boxSubscribers, Invitations = boxInvitations };
                return this.Json(new JsonResponse(true, response), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("GetBoxSubscribersAndInvitations user {0} box id {1}", this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with get friend list"), JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SharedBox(string boxId)
        {
            long boxid = ShortCodesCache.ShortCodeToLong(boxId);
            GetBoxQuery query = new GetBoxQuery(boxid, string.Empty);
            BoxDto result = null;

            string accessToken = TempData["accessToken"] as string;
            //ViewData["accessToken"] = accessToken;
            try
            {

                result = m_ZboxService.GetBox(query);
                ViewData["Error"] = TempData["Error"];

                ViewData["BoxOwner"] = result.BoxOwner;
                ViewData["boxid"] = result.BoxId;
                ViewData["shortUid"] = boxId;
                ViewData["BoxName"] = result.BoxName;
                if (TempData["Error"] != null)
                {
                    return View();
                }

            }
            catch (BoxAccessDeniedException)
            {
            }
            catch (BoxDoesntExistException)
            {
            }

            if (result != null)
            {
                BoxPrivacySettings privacySettings = (BoxPrivacySettings)result.PrivacySetting;

                switch (privacySettings)
                {
                    case BoxPrivacySettings.Public:
                        if (HttpContext.User.Identity.IsAuthenticated)
                        {
                            return RedirectToAction("SubscribeToSharedBox", "Collaboration", new { boxId = boxId }); ;
                        }
                        else
                        {
                            return View();
                        }
                    //in new design need to change this to a different view
                    case BoxPrivacySettings.InvitationOnly:
                        if (HttpContext.User.Identity.IsAuthenticated)
                        {
                            return RedirectToAction("SubscribeToSharedBox", "Collaboration", new { boxId = boxId }); ;
                        }
                        else
                        {
                            return View("SharedBox_MemberOnly");
                        }

                    //return RedirectToAction("SubscribeToSharedBox", new { boxId = boxId });
                    case BoxPrivacySettings.PasswordProtected:


                        // if access token is not empty or null
                        // it means the viewer has already authenticated and got an access token
                        // now we can either regenerate this token if its close to being expired
                        // or we will redirect back to password if access has expired already
                        if (!string.IsNullOrWhiteSpace(accessToken))
                        {

                            RegenerateBoxAccessTokenCommand command = new RegenerateBoxAccessTokenCommand(result.BoxId, accessToken);

                            try
                            {
                                RegenerateBoxAccessTokenCommandResult regeneratedTokenResult = m_ZboxService.RegenerateBoxAccessToken(command);
                                if (regeneratedTokenResult.IsTokenRegenerationRequired)
                                    return redirectToSharedBoxWithAccessToken(result.shortUid, regeneratedTokenResult.AccessToken);
                                else
                                    return View();
                            }
                            catch (BoxAccessTokenExpiredException)
                            {
                                this.ViewData["error"] = "Your access to this box has expired. Please enter the password again.";
                                // fall down to password view from here
                            }
                        }

                        return View("SharedBox_PasswordForm");
                }
            }

            return View("SharedBox_NotShared");
        }



        public ActionResult SharedBox_PasswordForm(string boxId, string password)
        {
            // int numOfTimeTried = 0;
            long boxid = ShortCodesCache.ShortCodeToLong(boxId);
            int? numOfTimeTried = Session[boxId.ToString()] as int?;
            if (numOfTimeTried.HasValue)
            {
                if (numOfTimeTried.Value > 3)
                  return  RedirectToAction("Index", "Home");
            }
            else
            {
                numOfTimeTried = 1;
            }
            BoxAuthenticationCommand command = new BoxAuthenticationCommand(boxid, password);
            try
            {
                if (string.IsNullOrEmpty(password))
                    throw new BoxAccessDeniedException();
                BoxAuthenticationCommandResult result = m_ZboxService.BoxAuthentication(command);
                return redirectToSharedBoxWithAccessToken(boxId, result.AccessToken);
            }
            catch (BoxAccessDeniedException)
            {
                Session[boxId.ToString()] = ++numOfTimeTried;
                this.ViewData["error"] = "Wrong password";
                ViewData["shortUid"] = boxId;
                return View();
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("SharedBox_PasswordForm boxId {0} password {1}", boxId, password));
                TraceLog.WriteError(ex);

                return View();
            }
        }

        private ActionResult redirectToSharedBoxWithAccessToken(string boxId, string accessToken)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SubscribeToSharedBox", "Collaboration", new { boxId = boxId }); ;
            }
            TempData["accessToken"] = accessToken;
            return RedirectToAction("SharedBox", new { boxId = boxId });
        }

        [Authorize]
        public ActionResult SubscribeToSharedBox(string boxId)
        {
            long boxid = ShortCodesCache.ShortCodeToLong(boxId);
            ViewData["boxid"] = boxId;
            try
            {
                MembershipUser membershipUser = Membership.GetUser(this.User.Identity.Name);

                SubscribeToSharedBoxCommand command = new SubscribeToSharedBoxCommand(ExtractUserID.GetUserEmailId(), boxid);
                SubscribeToSharedBoxCommandResult result = m_ZboxService.SubscribeToSharedBox(command);

                return RedirectToAction("Index", "Home").AddFragment(boxId.ToString());

            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                //this.ViewData["Error"] = ex.Message;
                return RedirectToAction("SharedBox", new { boxId = boxId });
                //return View("SharedBox");
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
                //this.ViewData["Error"] = ex.Message;
                return RedirectToAction("SharedBox", new { boxId = boxId });
                //return View("SharedBox");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                //this.ViewData["Error"] = ex.Message;
                TraceLog.WriteInfo(string.Format("SubscribeToSharedBox user {0} box id {1}", this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);

                return RedirectToAction("SharedBox", new { boxId = boxId });
            }
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangeBoxPrivacySettings(string boxId, string newSettingsString, string passwd)
        {
            try
            {
                long boxid = ShortCodesCache.ShortCodeToLong(boxId);
                MembershipUser membershipUser = Membership.GetUser(this.User.Identity.Name);
                BoxPrivacySettings newSettings = (BoxPrivacySettings)Enum.Parse(typeof(BoxPrivacySettings), newSettingsString, true);

                ChangeBoxPrivacySettingsCommand command = new ChangeBoxPrivacySettingsCommand(ExtractUserID.GetUserEmailId(), boxid, newSettings, passwd);
                ChangeBoxPrivacySettingsCommandResult result = m_ZboxService.ChangeBoxPrivacySettings(command);

                return this.Json(new JsonResponse(true, new { privacySettings = newSettings.ToString(), password = passwd }));
            }
            catch (Exception e)
            {
                TraceLog.WriteInfo(string.Format("ChangeBoxPrivacySettings user:{0} boxid:{1} newSettingString:{2} password:{3}", this.User.Identity.Name, boxId, newSettingsString, passwd));
                TraceLog.WriteError(e);
                return this.Json(new JsonResponse(false, "Problem with changing privacy settings"));
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ChangeBoxNotificationSettings(long boxId, string newSettingsString)
        {
            try
            {
                MembershipUser membershipUser = Membership.GetUser(this.User.Identity.Name);
                NotificationSettings newSettings = (NotificationSettings)Enum.Parse(typeof(NotificationSettings), newSettingsString, true);

                ChangeNotificationSettingsCommand command = new ChangeNotificationSettingsCommand(boxId, ExtractUserID.GetUserEmailId(), newSettings);
                ChangeNotificationSettingsCommandResult result = m_ZboxService.ChangeNotificationSettings(command);

                return this.Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("ChangeBoxNotificationSettings user:{0} boxid:{1} newSettingsString:{2} ", this.User.Identity.Name, boxId, newSettingsString));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "Problem with change notification setting"));
            }
        }

    }
}
