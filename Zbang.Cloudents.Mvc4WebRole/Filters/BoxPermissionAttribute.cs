using System;
using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class BoxPermissionAttribute : ActionFilterAttribute
    {
        private readonly string m_BoxParamsId;
        public BoxPermissionAttribute(string boxParamsId)
        {
            m_BoxParamsId = boxParamsId;
        }

        //[Dependency]
        public IZboxReadSecurityReadService ZboxReadService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var boxid = filterContext.ActionParameters[m_BoxParamsId];
            var boxId = Convert.ToInt64(boxid);
            var userId = filterContext.HttpContext.User.GetUserId(false);

            var inviteId = filterContext.HttpContext.Request.QueryString["invId"];
            var guid = GuidEncoder.TryParseNullableGuid(inviteId);
            var cookieHelper = new CookieHelper(filterContext.HttpContext);
            var cookie = cookieHelper.ReadCookie<Permission>(Permission.CookieName);
            UserRelationshipType userType;

            if (cookie == null || cookie.Expire < DateTime.UtcNow || cookie.BoxId != boxId || cookie.UserId != userId)
            {
                try
                {
                    userType = ZboxReadService.GetUserStatusToBox(boxId, userId, guid);
                }
                catch (BoxDoesntExistException)
                {
                    filterContext.Result = new HttpStatusCodeResult(404);
                    return;
                }
                catch (BoxAccessDeniedException)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new HttpUnauthorizedResult();
                        return;
                    }
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"action", "MembersOnly"},
                        {"controller", "Error"},
                        {"returnUrl", filterContext.HttpContext.Request.Url?.AbsolutePath},
                        {"invId", filterContext.HttpContext.Request.QueryString["invId"]}
                    });
                    return;
                }

                cookieHelper.InjectCookie(Permission.CookieName,
                    new Permission
                    {
                        BoxId = boxId,
                        Expire = DateTime.UtcNow.AddMinutes(20),
                        UserId = userId,
                        UserType = userType
                    });
            }
            else
            {
                userType = cookie.UserType;
            }

            filterContext.Controller.ViewBag.UserType = userType;
            base.OnActionExecuting(filterContext);
        }

        public class Permission
        {
            public const string CookieName = "p";
            public long BoxId { get; set; }
            public DateTime Expire { get; set; }
            public long UserId { get; set; }
            public UserRelationshipType UserType { get; set; }
        }
    }
}