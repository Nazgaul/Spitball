using System;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class BoxPermissionAttribute : ActionFilterAttribute
    {
        private readonly string m_BoxParamsId;
        public BoxPermissionAttribute(string boxParamsId)
        {
            m_BoxParamsId = boxParamsId;
        }

        [Dependency]
        public IZboxReadSecurityReadService ZboxReadService { get; set; }

        private const string CookieName = "p";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var boxid = filterContext.ActionParameters[m_BoxParamsId];
            var boxId = Convert.ToInt64(boxid);
            var userId = filterContext.HttpContext.User.GetUserId(false);

            var cookieHelper = new CookieHelper(filterContext.HttpContext);
            var cookie = cookieHelper.ReadCookie<Permission>(CookieName);
            UserRelationshipType userType;

            if (cookie == null || cookie.Expire < DateTime.UtcNow || cookie.BoxId != boxId || cookie.UserId != userId)
            {
                try
                {
                    userType = ZboxReadService.GetUserStatusToBox(boxId, userId);
                }
                catch (BoxDoesntExistException ex)
                {
                    TraceLog.WriteError("Box Index desktop", ex);
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"action", "Index"},
                        {"controller", "Error"}
                    });
                    return;
                }
                catch (BoxAccessDeniedException)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"action", "MembersOnly"},
                        {"controller", "Error"},
                        {"returnUrl", filterContext.HttpContext.Request.Url != null ? filterContext.HttpContext.Request.Url.AbsolutePath : null}
                    });
                    return;
                }

                cookieHelper.InjectCookie(CookieName,
                    new Permission
                    {
                        BoxId = boxId,
                        Expire = DateTime.UtcNow.AddMinutes(5),
                        UserId = userId,
                        UserType = UserRelationshipType.Owner
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
            public long BoxId { get; set; }
            public DateTime Expire { get; set; }
            public long UserId { get; set; }
            public UserRelationshipType UserType { get; set; }
        }
    }
}