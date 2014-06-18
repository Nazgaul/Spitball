using System;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    /// <summary>
    /// Like Authorize attribute only when user is not in role redirect to verify email
    /// </summary>
    public class ZboxAuthorizeAttribute : AuthorizeAttribute
    {
        //private string[] _rolesSplit = new string[0];
        //private string[] _usersSplit = new string[0];
        //private bool userValidationNotPassed = false;
        //private bool m_UserNotVerified = false;

        //private readonly IZboxReadService m_ZboxReadService;
        //private readonly IFormsAuthenticationService m_FormsAuthenticationSerivce;

        public ZboxAuthorizeAttribute()
        {
            IsAuthenticationRequired = true;
            //m_ZboxReadService = DependencyResolver.Current.GetService<IZboxReadService>();
            //var m_FormsAuthenticationSerivce = DependencyResolver.Current.GetService<Zbang.Zbox.Infrastructure.Security.IFormsAuthenticationService>();
        }
        /// <summary>
        /// should user need to be autherise - default is true
        /// </summary>
        public bool IsAuthenticationRequired { get; set; }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            // var id = (httpContext.Request.RequestContext.RouteData.Values["id"] as string)
            //??
            //(httpContext.Request["id"] as string);
            var result = base.AuthorizeCore(httpContext);
            return result;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!IsAuthenticationRequired) return;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //var headerSend = filterContext.HttpContext.Items[HTTPItemConsts.HeaderSend];
                //if (headerSend != null && (bool)headerSend)
                //{
                try
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    //401 asp.net get this status code and redirect to login page                
                    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    filterContext.HttpContext.Response.Flush();
                    filterContext.Result = new HttpStatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                    filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                    filterContext.HttpContext.Response.End();
                }
                catch (Exception ex)
                {
                    Zbox.Infrastructure.Trace.TraceLog.WriteError("Zbox Authorize Attribute trying to send header", ex);

                }
                //}
                filterContext.Result = new EmptyResult();
                return;
            }

            // base.HandleUnauthorizedRequest(filterContext);
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}