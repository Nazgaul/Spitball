using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Zbang.Cloudents.Mvc4WebRole.Controllers.Resources;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Share;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ShareController : BaseController
    {

        [HttpPost, ZboxAuthorize,ActionName("Invite")]
        public async Task<JsonResult> InviteAsync(InviteSystem model)
        {
            try
            {
                if (model == null)
                {
                    return JsonError("no data");
                }
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
                TraceLog.WriteError($"Share/Invite user: {User.GetUserId()} model: {model}", ex);
                return JsonError(BaseControllerResources.UnspecifiedError);
            }
        }
        

        [HttpPost]
        [ZboxAuthorize,ActionName("InviteBox")]
        public async Task<JsonResult> InviteBoxAsync(Invite model)
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
                ModelState.AddModelError(string.Empty, ShareControllerResources.ShareController_InviteBox_You_do_not_have_permission_to_share_a_box);
                TraceLog.WriteError($"InviteBox user: {User.GetUserId()} model: {model}", ex);
                return JsonError(GetErrorFromModelState());
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return JsonError(GetErrorFromModelState());
            }
            catch (Exception ex)
            {

                TraceLog.WriteError($"InviteBox user: {User.GetUserId()} model: {model}", ex);
                ModelState.AddModelError(string.Empty, BaseControllerResources.UnspecifiedError);
                return JsonError(GetErrorFromModelState());
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        [RemoveBoxCookie, ActionName("SubscribeToBox")]
        public JsonResult SubscribeToBox(long boxId)
        {
            var userid = User.GetUserId();
            try
            {
                var command = new SubscribeToSharedBoxCommand(userid, boxId);
                ZboxWriteService.SubscribeToSharedBox(command);
                return JsonOk();
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"SubscribeToBox userid {userid} boxid {boxId}", ex);
                return JsonError();
            }

        }

        //[HttpGet,ActionName("FromEmail")]
        //public async Task<ActionResult> FromEmailAsync(string key, string email)
        //{
        //    var membersOnlyErrorPageRedirect = RedirectToAction("MembersOnly", "Error");
        //    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(email))
        //    {
        //        return membersOnlyErrorPageRedirect;
        //    }
        //    try
        //    {
        //        var values = m_InviteLinkDecrypt.Value.DecryptInviteUrl(key, email);


        //        if (values.ExpireTime < DateTime.UtcNow || values.RecipientEmail != email)
        //        {
        //            return membersOnlyErrorPageRedirect;
        //        }
        //        var inviteExists = await ZboxReadService.GetInviteAsync(new GetInviteDetailQuery(values.Id));
        //        if (!inviteExists)
        //        {
        //            return membersOnlyErrorPageRedirect;
        //        }
        //        if (!string.IsNullOrEmpty(values.BoxUrl)) return Redirect(values.BoxUrl);
        //        var boxId = m_ShortCodesCache.Value.LongToShortCode(values.BoxId);
        //        var urlToRedirect = Url.ActionLinkWithParam("Index", "Box", new { boxId });
        //        return Redirect(urlToRedirect);
        //    }
        //    //we are here if user is not register to the system
        //    catch (UserNotFoundException)
        //    {
        //        return membersOnlyErrorPageRedirect;
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError($"FromEmail key:{key} email:{email}", ex);
        //        return membersOnlyErrorPageRedirect;

        //    }
        //}
        [ZboxAuthorize, HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult ShareDialog()
        {
            return PartialView("ShareDialog");
        }

        [ZboxAuthorize, HttpGet]
        [DonutOutputCache(CacheProfile = "PartialPage")]
        public ActionResult InviteDialog()
        {
            return PartialView();
        }

        //[ZboxAuthorize,HttpPost,ActionName("Index")]
        //public async Task<JsonResult> IndexAsync()
        //{
        //    var command = new AddReputationCommand(User.GetUserId(),
        //        Zbox.Infrastructure.Enums.ReputationAction.ShareFacebook);
        //    await ZboxWriteService.AddReputationAsync(command);
        //    return JsonOk();
        //}
    }
}
