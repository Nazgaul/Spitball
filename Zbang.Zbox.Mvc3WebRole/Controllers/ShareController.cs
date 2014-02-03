using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using DevTrends.MvcDonutCaching;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Mvc3WebRole.Attributes;
using Zbang.Zbox.Mvc3WebRole.Helpers;
using Zbang.Zbox.Mvc3WebRole.Models;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using System.Web.Routing;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Zbox.Mvc3WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class ShareController : BaseController
    {
        protected const string SessionInviteKey = "Invites";

        public ShareController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IShortCodesCache shortToLongCache,
            IFormsAuthenticationService formsAuthenticationService)
            : base(zboxWriteService, zboxReadService, shortToLongCache, formsAuthenticationService)
        { }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult InviteBox(Message model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, GetModelStateErrors()));
                }
                if (string.IsNullOrWhiteSpace(model.BoxUid))
                {
                    return Json(new JsonResponse(false, "BoxUid is required"));
                }

                //var privacySettingChanged = false;

                var userId = GetUserId();
                var boxId = m_ShortToLongCode.ShortCodeToLong(model.BoxUid);
                var query = new GetBoxQuery(boxId, userId);
                var boxSettingResult = m_ZboxReadService.GetBoxSetting(query);
                if (boxSettingResult.PrivacySetting == BoxPrivacySettings.Private)
                {
                    var boxPrivacyCommand = new ChangeBoxPrivacySettingsCommand(userId, boxId, BoxPrivacySettings.MembersOnly, string.Empty);
                    m_ZboxWriteService.ChangeBoxPrivacySettings(boxPrivacyCommand);
                }
                var shareCommand = new ShareBoxCommand(boxId, userId, model.Recepients, model.Note);
                m_ZboxWriteService.ShareBox(shareCommand);

                //RemoveBoxUserCaching(userId);
                return Json(new JsonResponse(true));
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, "You do not have permission to share a box");
                TraceLog.WriteError(string.Format("InviteBox user: {0} model: {1}", GetUserId(), model.ToString()), ex);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
            catch (Exception ex)
            {

                TraceLog.WriteError(string.Format("InviteBox user: {0} model: {1}", GetUserId(), model.ToString()), ex);
                ModelState.AddModelError(string.Empty, "Unsepcified error. try again later");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult MailShare(Message model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new JsonResponse(false, GetModelStateErrors()));
                }
                if (string.IsNullOrWhiteSpace(model.BoxUid))
                {
                    return Json(new JsonResponse(false, "BoxUid is required"));
                }
                var userId = GetUserId();
                var boxId = m_ShortToLongCode.ShortCodeToLong(model.BoxUid);
                var url = string.Empty;
                if (string.IsNullOrEmpty(model.ItemUid))
                {
                    url = Url.Action("Index", "Box",
                        new RouteValueDictionary(new { BoxUid = model.BoxUid })
                        , Request.Url.Scheme, null);
                }
                else
                {
                    url = Url.Action("Index", "Item",
                        new RouteValueDictionary(new { BoxUid = model.BoxUid, ItemUid = model.ItemUid })
                        , Request.Url.Scheme, null);
                }

                var command = new SendMessageToEmailCommand(userId, model.Recepients,
                    model.Note, url);
                m_ZboxWriteService.SendMessage(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("SendMessage user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, "Unsepcified error. try again later");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
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
                var command = new SendMessageToEmailCommand(userId, model.Recepients,
                        model.Note, string.Empty);
                m_ZboxWriteService.SendMessage(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("SendMessage user: {0} model: {1}", GetUserId(), model), ex);
                ModelState.AddModelError(string.Empty, "Unsepcified error. try again later");
                return Json(new JsonResponse(false, GetModelStateErrors()));
            }
        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult SubscribeToBox(string BoxUid)
        {
            var boxid = m_ShortToLongCode.ShortCodeToLong(BoxUid);
            var userid = GetUserId();
            var command = new SubscribeToSharedBoxCommand(userid, boxid);
            m_ZboxWriteService.SubscribeToSharedBox(command);
            //RemoveInvitesFromSession();
            return Json(new JsonResponse(true));

        }

        [HttpPost]
        [ZboxAuthorize]
        public ActionResult DeclineInvatation(string BoxUid)
        {
            try
            {
                var boxid = m_ShortToLongCode.ShortCodeToLong(BoxUid);
                var userid = GetUserId();
                var command = new DeleteUserFromBoxCommand(userid, userid, boxid);
                m_ZboxWriteService.DeleteUserFromBox(command);
                //RemoveInvitesFromSession();
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("DeclineInvatation user: {0} BoxUid: {1}", GetUserId(), BoxUid), ex);
                return Json(new JsonResponse(false));
            }
        }

        [ZboxAuthorize(IsAuthenticationRequired = false)]//we need that because of verify account this happen - so infinite loop
        [OutputCache(Duration = TimeConsts.Minute, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        [HttpGet]
        [Ajax]
        public ActionResult Invites()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new JsonResponse(true, new string[0]), JsonRequestBehavior.AllowGet);
            }
            var userid = GetUserId();
            try
            {
                //var invites = Session[SessionInviteKey] as IEnumerable<InviteDto>;
                //if (invites == null)
                //{
                    var query = new GetInvitesQuery(userid);
                   var invites = m_ZboxReadService.GetInvites(query);
                   // Session[SessionInviteKey] = invites;
                //}
                return Json(new JsonResponse(true, invites), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //RemoveInvitesFromSession();
                //RemoveInvitesFromSession();
                TraceLog.WriteError("Share Invites userid " + userid, ex);
                return Json(new JsonResponse(true, new string[0]), JsonRequestBehavior.AllowGet);
            }
        }

        //[NonAction]
        //protected void RemoveInvitesFromSession()
        //{
        //    Session.Remove(SessionInviteKey);
        //}
    }
}
