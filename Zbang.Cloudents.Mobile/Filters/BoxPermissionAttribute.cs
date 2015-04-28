﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mobile.Filters
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
                catch (BoxDoesntExistException)
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new HttpStatusCodeResult(404);
                        return;
                    }
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"action", "Index"},
                        {"controller", "Error"}
                    });
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
            public long BoxId { get; set; }
            public DateTime Expire { get; set; }
            public long UserId { get; set; }
            public UserRelationshipType UserType { get; set; }
        }
    }

    public class RemoveBoxCookieAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookieHelper = new CookieHelper(filterContext.HttpContext);
            cookieHelper.RemoveCookie("p");
            base.OnActionExecuting(filterContext);
        }
    }
}