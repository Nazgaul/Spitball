﻿using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Cloudents.Mvc4WebRole.Extensions;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class NoUniversityAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var universityId = filterContext.HttpContext.User.GetUniversityId();

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }
            //var userData = m_FormsAuthenticationService.GetUserData();
            //if (userData == null)
            //{
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}
            if (universityId.HasValue)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new EmptyResult();
                //new JsonResult { Data = new JsonResponse(false),
                //    JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                   { "controller", "Library" }, { "action", "Choose" }, 
                   //{ "returnUrl",filterContext.HttpContext.Request.QueryString["returnUrl"]},
                   { "new", filterContext.HttpContext.Request.QueryString["new"]}

               });
        }
    }
}