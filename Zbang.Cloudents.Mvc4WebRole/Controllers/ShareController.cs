using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.Share;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ShareController : BaseController
    {
        private readonly Lazy<IInviteLinkDecrypt> m_InviteLinkDecrypt;
        private readonly Lazy<IShortCodesCache> m_ShortCodesCache;

        public ShareController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            Lazy<IShortCodesCache> shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService,
            Lazy<IInviteLinkDecrypt> inviteLinkDecrypt)
            : base(zboxWriteService, zboxReadService,

            formsAuthenticationService)
        {
            m_InviteLinkDecrypt = inviteLinkDecrypt;
            m_ShortCodesCache = shortToLongCache;
        }

        [ZboxAuthorize, HttpGet]
        [CompressFilter]
        public ActionResult Index(long? boxid)
        {
            var model = new Zbox.ViewModel.DTOs.BoxDtos.BoxMetaDto();
            try
            {
                if (boxid.HasValue)
                {
                    model = m_ZboxReadService.GetBoxMeta(new GetBoxQuery(boxid.Value, GetUserId()));
                    var urlBuilder = new UrlBuilder(HttpContext);
                    model.Url = urlBuilder.BuildBoxUrl(model.Id, model.Name, model.UnivtesityName);
                }
            }
            catch (BoxAccessDeniedException)
            {
                if (Request.IsAjaxRequest())
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                }
                return RedirectToAction("MembersOnly", "Error");
            }
            catch (BoxDoesntExistException)
            {
                if (Request.IsAjaxRequest())
                {
                    return HttpNotFound();
                }
                return RedirectToAction("Index", "Error");
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpPost, ZboxAuthorize]
        public ActionResult Invite(InviteSystem model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, GetModelStateErrors()));
                }
                var userId = GetUserId();

                var inviteCommand = new InviteToSystemCommand(userId, model.Recepients);
                m_ZboxWriteService.InviteSystem(inviteCommand);

                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Share/Invite user: {0} model: {1}", GetUserId(), model), ex);
                return Json(new JsonResponse(false, "Unsepcified error. try again later"));
            }
        }
        [HttpPost, ZboxAuthorize]
        public ActionResult InviteFacebook(InviteSystemFromFacebook model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, GetModelStateErrors()));
                }
                var userId = GetUserId();

                var inviteCommand = new InviteToSystemFacebookCommand(userId, model.Id, model.UserName, model.FirstName, model.MiddleName, model.LastName, model.Sex);
                m_ZboxWriteService.InviteSystemFromFacebook(inviteCommand);

                return Json(new JsonResponse(true));
             }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Share/InviteFacebook user: {0} model: {1}", GetUserId(), model), ex);
                return Json(new JsonResponse(false, "Unsepcified error. try again later"));
            }
        }


        [HttpPost]
        [ZboxAuthorize]
        public ActionResult InviteBox(Invite model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, GetModelStateErrors()));
                }


                var userId = GetUserId();
                var shareCommand = new ShareBoxCommand(model.BoxUid, userId, model.Recepients);
                m_ZboxWriteService.ShareBox(shareCommand);
                return Json(new JsonResponse(true));
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, "You do not have permission to share a box");
                TraceLog.WriteError(string.Format("InviteBox user: {0} model: {1}", GetUserId(), model), ex);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            catch (Exception ex)
            {

                TraceLog.WriteError(string.Format("InviteBox user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, "Unsepcified error. try again later");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }

        [HttpPost, ZboxAuthorize]
        public ActionResult InviteBoxFacebook(InviteToBoxFromFacebook model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            var userId = GetUserId();
            var command = new ShareBoxFacebookCommand(userId, model.Id, model.UserName, model.BoxId, model.FirstName, model.MiddleName, model.LastName, model.Sex);
            m_ZboxWriteService.ShareBoxFacebbok(command);
            return Json(new JsonResponse(true));
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult Message(Message model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, GetModelStateErrors()));
                }
                var userId = GetUserId();
                var command = new SendMessageCommand(userId, model.Recepients,
                        model.Note);
                m_ZboxWriteService.SendMessage(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("SendMessage user: {0} model: {1}", GetUserId(), model), ex);
                return Json(new JsonResponse(false, "Unsepcified error. try again later"));
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult SubscribeToBox(long boxUid)
        {
            var userid = GetUserId();
            try
            {

                var command = new SubscribeToSharedBoxCommand(userid, boxUid);
                m_ZboxWriteService.SubscribeToSharedBox(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("SubscribeToBox userid {0} boxid {1}", userid, boxUid), ex);
                return Json(new JsonResponse(false));
            }

        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult DeclineInvatation(long boxUid)
        {
            try
            {
                var userid = GetUserId();
                var command = new DeleteUserFromBoxCommand(userid, userid, boxUid);
                m_ZboxWriteService.DeleteUserFromBox(command);
                //RemoveInvitesFromSession();
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeclineInvatation user: {0} BoxUid: {1}", GetUserId(), boxUid), ex);
                return Json(new JsonResponse(false));
            }
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]//we need that because of verify account this happen - so infinite loop
        [OutputCache(Duration = TimeConsts.Minute, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        [CompressFilter]
        [HttpGet]
        [Ajax]
        [AjaxCache(TimeToCache = 0)]
        public async Task<ActionResult> Notifications(int page = 0)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return this.CdJson(new JsonResponse(true, new string[0]));
            }
            var userid = GetUserId();
            try
            {
                var query = new GetInvitesQuery(userid, page);
                var invites = await m_ZboxReadService.GetInvites(query);

                var urlBuilder = new UrlBuilder(HttpContext);
                return this.CdJson(new JsonResponse(true, invites.Select(s =>
                {
                    if (s.BoxId.HasValue)
                    {
                        s.Url = urlBuilder.BuildBoxUrl(s.BoxId.Value, s.BoxName, s.Universityname);
                    }
                    return s;
                })));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Share Notifications userid " + userid, ex);
                return this.CdJson(new JsonResponse(true, new string[0]));
            }
        }


        [ChildActionOnly]
        [ActionName("Invites")]
        public ActionResult Invites()
        {
            return PartialView();
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


                if (values.ExpireTime < DateTime.UtcNow || values.RecepientEmail != email)
                {
                    return membersOnlyErrorPageRedirect;
                }
                var inviteExists = await m_ZboxReadService.GetInvite(new GetInviteDetailQuery(values.Id));
                if (!inviteExists)
                {
                    return membersOnlyErrorPageRedirect;
                }

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
        public ActionResult Facebook(string postId)
        {
            if (string.IsNullOrWhiteSpace(postId))
            {
                return Json(new JsonResponse(false));
            }
            var command = new AddReputationCommand(GetUserId());
            m_ZboxWriteService.AddReputation(command);
            return Json(new JsonResponse(true));
        }

        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult NotificationAsRead(Guid messageId)
        {
            var command = new MarkMessagesAsReadCommand(GetUserId(), messageId);
            m_ZboxWriteService.MarkMessageAsRead(command);
            return this.CdJson(new JsonResponse(true));
        }

        [HttpPost, Ajax, ZboxAuthorize]
        public ActionResult NotificationOld()
        {
            var command = new MarkMessagesAsOldCommand(GetUserId());
            m_ZboxWriteService.MarkMessagesAsOld(command);
            return this.CdJson(new JsonResponse(true));
        }
    }
}
