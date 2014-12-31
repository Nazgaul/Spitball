﻿using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.Mobile.Filters
{
    public class NoUniversityAttribute : ActionFilterAttribute
    {
        private readonly IFormsAuthenticationService m_FormsAuthenticationService;

        public NoUniversityAttribute()
        {
            m_FormsAuthenticationService = DependencyResolver.Current.GetService<IFormsAuthenticationService>();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var userData = m_FormsAuthenticationService.GetUserData();
            if (userData == null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            if (userData.UniversityId.HasValue)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult { Data = new JsonResponse(false), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                   { "controller", "Library" }, { "action", "Choose" }, 
                   { "returnUrl",filterContext.HttpContext.Request.QueryString["returnUrl"]},
                   { "new", filterContext.HttpContext.Request.QueryString["new"]}

               });
        }
    }
}