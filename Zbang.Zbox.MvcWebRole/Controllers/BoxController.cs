using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.MvcWebRole.Helpers;
using System.Diagnostics;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.MvcWebRole.Controllers
{
    public class BoxController : Controller
    {
        private IZboxService m_ZboxService;

        public BoxController(IZboxService zboxService)
        {
            m_ZboxService = zboxService;
        }
        [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetBoxes()
        {
            try
            {
                string userEmailId = ExtractUserID.GetUserEmailId();
                var cachedobj = GetUserBoxData(userEmailId);
                return this.Json(cachedobj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                TraceLog.WriteInfo(string.Format("GetBoxes user {0} ", this.User.Identity.Name));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "Problem with getting boxes"), JsonRequestBehavior.AllowGet);
            }
        }

        private JsonResponse GetUserBoxData(string emailId)
        {
            GetBoxesQuery queryGetBoxes = new GetBoxesQuery(emailId);
            IList<UserBoxesDto> boxDtos = m_ZboxService.GetBoxes(queryGetBoxes);

            var ownedBox = boxDtos.Where(w => w.UserType == UserType.owner);
            var subscribedBox = boxDtos.Where(w => w.UserType == UserType.subscribe);
            var invitedBox = boxDtos.Where(w => w.UserType == UserType.invite);
            return new JsonResponse(true, new { ownedBoxes = ownedBox, subscribedBox = subscribedBox, invitedBoxes = invitedBox });
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CreateBox(string newBoxName, int notificationSettings, int privacySettings, string privacyPassword)
        {
            try
            {
                string userEmailId = ExtractUserID.GetUserEmailId();
                BoxPrivacySettings newBoxPrivacySettings = (BoxPrivacySettings)privacySettings;
                NotificationSettings newBoxNotificationSettings = (NotificationSettings)notificationSettings;
                CreateBoxCommand command = new CreateBoxCommand(userEmailId, newBoxName, newBoxNotificationSettings, newBoxPrivacySettings, privacyPassword);

                CreateBoxCommandResult result = m_ZboxService.CreateBox(command);

                UserBoxesDto newBox = new UserBoxesDto()
                {
                    BoxId = result.NewBox.Id,
                    BoxName = result.NewBox.BoxName,
                    //uId = result.NewBox.uId,
                    BoxOwner = "You",
                    //PrivacyPassword = result.NewBox.PrivacySettings.Password,
                    PrivacySetting = result.NewBox.PrivacySettings.PrivacySetting,
                    UserPermission = (int)UserPermissionSettings.Owner,
                    UserType = UserType.owner,
                    //NotificationSettings = newBoxNotificationSettings
                };

                return this.Json(new JsonResponse(true, newBox));
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(string.Format("CreateBox user {0} box name {1} notification settings {2} privacy settings {3} privacy password {4} ", this.User.Identity.Name, newBoxName, notificationSettings, privacySettings, privacyPassword));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "Problem with creating a box"));
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ChangeBoxName(string boxId, string newBoxName)
        {
            try
            {
                string userEmailId = ExtractUserID.GetUserEmailId();
                var boxid = ShortCodesCache.ShortCodeToLong(boxId);
                ChangeBoxNameCommand command = new ChangeBoxNameCommand(boxid, userEmailId, newBoxName);
                ChangeBoxNameCommandResult result = m_ZboxService.ChangeBoxName(command);

                return this.Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(string.Format("ChangeBoxName user {0} boxid {1} newBoxName {2} ", this.User.Identity.Name, boxId, newBoxName));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "Problem with change box name"));
            }
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteBox(string boxId)
        {
            try
            {
                var userEmailId = ExtractUserID.GetUserEmailId();
                var boxid = ShortCodesCache.ShortCodeToLong(boxId);
                DeleteBoxCommand command = new DeleteBoxCommand(boxid, userEmailId);
                DeleteBoxCommandResult result = m_ZboxService.DeleteBox(command);

                return this.Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(string.Format("DeleteBox user {0} boxid {1}", this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);
                return this.Json(new JsonResponse(false, "Problem with deleting a box"));


            }
        }

        [HttpGet]
        [CompressFilter]
        [Authorize]
        public JsonResult GetBoxData(string boxId)
        {
            try
            {

                var BoxId = ShortCodesCache.ShortCodeToLong(boxId);
                string userEmailId = ExtractUserID.GetUserEmailId();

                GetBoxUsersQuery UsersQuery = new GetBoxUsersQuery(BoxId, userEmailId);
                var boxUsers = m_ZboxService.GetBoxUsers(UsersQuery);

                var boxSubscribers = boxUsers.Where(w => w.UserType == UserType.subscribe);
                var boxInvitations = boxUsers.Where(w => w.UserType == UserType.invite);

                GetBoxCommentsQuery commentQuery = new GetBoxCommentsQuery(BoxId, userEmailId);
                var commentDtos = m_ZboxService.GetBoxComments(commentQuery);

                GetBoxItemsPagedQuery itemsQuery = new GetBoxItemsPagedQuery(BoxId, userEmailId);
                var boxItemDtos = m_ZboxService.GetBoxItemsPaged(itemsQuery);

                var boxUserResponse = new { Subscribers = boxSubscribers, Invitations = boxInvitations };
                var commentResponse = new { boxId = boxId, comments = commentDtos };
                var itemsResponse = new { boxId = boxId, boxItemDtos = boxItemDtos };

                var response = new { boxUser = boxUserResponse, boxComment = commentResponse, boxItem = itemsResponse };

                return this.Json(new JsonResponse(true, response), JsonRequestBehavior.AllowGet);
            }
            catch (BoxAccessDeniedException ex)
            {
                TraceLog.WriteInfo(string.Format("GetBoxData BoxAccessDeniedException user {0} box id {1}", this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with get friend list"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                TraceLog.WriteInfo(string.Format("GetBoxData user {0} box id {1}", this.User.Identity.Name, boxId));
                TraceLog.WriteError(ex);

                return this.Json(new JsonResponse(false, "Problem with get friend list"), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
