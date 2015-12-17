﻿using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Share;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ShareController : BaseController
    {
        private readonly Lazy<IInviteLinkDecrypt> m_InviteLinkDecrypt;
        private readonly Lazy<IShortCodesCache> m_ShortCodesCache;


        public ShareController(
            Lazy<IShortCodesCache> shortToLongCache,
            Lazy<IInviteLinkDecrypt> inviteLinkDecrypt)
        {
            m_InviteLinkDecrypt = inviteLinkDecrypt;
            m_ShortCodesCache = shortToLongCache;
        }


        [HttpPost, ZboxAuthorize]
        public async Task<JsonResult> Invite(InviteSystem model)
        {
            try
            {
                if (model.Recipients.Length == 0)
                {
                    return JsonError("recipients is empty");
                }
                var userId = User.GetUserId();

                var inviteCommand = new InviteToSystemCommand(userId, model.Recipients);
                await ZboxWriteService.InviteSystemAsync(inviteCommand);

                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Share/Invite user: {0} model: {1}", User.GetUserId(), model), ex);
                return JsonError("Unspecified error. try again later");
            }
        }
        

        [HttpPost]
        [ZboxAuthorize]
        public async Task<JsonResult> InviteBox(Invite model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return JsonError(GetErrorFromModelState());
                }

                var userId = User.GetUserId();
                var shareCommand = new ShareBoxCommand(model.BoxId, userId, model.Recipients);
                await ZboxWriteService.ShareBoxAsync(shareCommand);
                return JsonOk();
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, @"You do not have permission to share a box");
                TraceLog.WriteError(string.Format("InviteBox user: {0} model: {1}", User.GetUserId(), model), ex);
                return JsonError(GetErrorFromModelState());
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return JsonError(GetErrorFromModelState());
            }
            catch (Exception ex)
            {

                TraceLog.WriteError(string.Format("InviteBox user: {0} model: {1}", User.GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, @"Unspecified error. try again later");
                return JsonError(GetErrorFromModelState());
            }
        }

        

        //[HttpPost]
        //[ZboxAuthorize]
        //public async Task<JsonResult> Message(Message model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return JsonError(GetErrorFromModelState());
        //        }
        //        var userId = User.GetUserId();
        //        var command = new SendMessageCommand(userId, model.Recepients,
        //                model.Note);
        //        await ZboxWriteService.SendMessageAsync(command);
        //        return JsonOk();
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError(string.Format("SendMessage user: {0} model: {1}", User.GetUserId(), model), ex);
        //        return JsonError("Unspecified error. try again later");
        //    }
        //}

       

        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie]
        public async Task<JsonResult> SubscribeToBox(long boxId)
        {
            var userid = User.GetUserId();
            try
            {
                var command = new SubscribeToSharedBoxCommand(userid, boxId);
                await ZboxWriteService.SubscribeToSharedBoxAsync(command);
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("SubscribeToBox userid {0} boxid {1}", userid, boxId), ex);
                return JsonError();
            }

        }



        [ZboxAuthorize(IsAuthenticationRequired = false)]//we need that because of verify account this happen - so infinite loop
        //[OutputCache(Duration = TimeConsts.Minute, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        [HttpGet]
        public async Task<ActionResult> Notifications()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return JsonOk(new string[0]);
            }
            var userid = User.GetUserId();
            try
            {
                var query = new GetInvitesQuery(userid);
                var invites = await ZboxReadService.GetInvitesAsync(query);
                return JsonOk(invites);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Share Notifications userid " + userid, ex);
                return JsonOk(new string[0]);
            }
        }


       

        [HttpGet]
        public async Task<ActionResult> FromEmail(string key, string email)
        {
            var membersOnlyErrorPageRedirect = RedirectToAction("MembersOnly", "Error");
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(email))
            {
                return membersOnlyErrorPageRedirect;
            }
            try
            {
                var values = m_InviteLinkDecrypt.Value.DecryptInviteUrl(key, email);


                if (values.ExpireTime < DateTime.UtcNow || values.RecipientEmail != email)
                {
                    return membersOnlyErrorPageRedirect;
                }
                var inviteExists = await ZboxReadService.GetInviteAsync(new GetInviteDetailQuery(values.Id));
                if (!inviteExists)
                {
                    return membersOnlyErrorPageRedirect;
                }
                if (!string.IsNullOrEmpty(values.BoxUrl)) return Redirect(values.BoxUrl);
                var boxUid = m_ShortCodesCache.Value.LongToShortCode(values.BoxId);
                var urlToRedirect = Url.ActionLinkWithParam("Index", "Box", new { boxUid });
                return Redirect(urlToRedirect);
            }
            //we are here if user is not register to the system
            catch (UserNotFoundException)
            {
                return membersOnlyErrorPageRedirect;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("FromEmail key:{0} email:{1}", key, email), ex);
                return membersOnlyErrorPageRedirect;

            }
        }



        [HttpPost]
        [ZboxAuthorize]
        public async Task<JsonResult> Facebook(string postId)
        {
            if (string.IsNullOrWhiteSpace(postId))
            {
                return JsonError();
            }
            var command = new AddReputationCommand(User.GetUserId(), Zbox.Infrastructure.Enums.ReputationAction.ShareFacebook);
            await ZboxWriteService.AddReputationAsync(command);
            return JsonOk();
        }

        [HttpPost, ZboxAuthorize]
        public ActionResult NotificationAsRead(Guid messageId)
        {
            var command = new MarkMessagesAsReadCommand(User.GetUserId(), messageId);
            ZboxWriteService.MarkMessageAsRead(command);
            return Json(new JsonResponse(true));
        }

        
        [HttpPost, ZboxAuthorize]
        public ActionResult NotificationDelete(Guid messageId)
        {
            var command = new DeleteNotificationCommand(messageId);
            ZboxWriteService.DeleteNotification(command);
            return JsonOk();
        }
    }
}
