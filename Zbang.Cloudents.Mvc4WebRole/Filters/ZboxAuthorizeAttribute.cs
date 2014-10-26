using System;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    /// <summary>
    /// Like Authorize attribute only when user is not in role redirect to verify email
    /// </summary>
    public class ZboxAuthorizeAttribute : AuthorizeAttribute
    {

        public ZboxAuthorizeAttribute()
        {
            IsAuthenticationRequired = true;
        }
        /// <summary>
        /// should user need to be Authorize - default is true
        /// </summary>
        public bool IsAuthenticationRequired { get; set; }

     

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!IsAuthenticationRequired) return;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                try
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    
                    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    filterContext.HttpContext.Response.Flush();
                    filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
                    filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
                    filterContext.HttpContext.Response.End();
                }
                catch (Exception ex)
                {
                    Zbox.Infrastructure.Trace.TraceLog.WriteError("Zbox Authorize Attribute trying to send header", ex);

                }
                filterContext.Result = new EmptyResult();
                return;
            }

            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}