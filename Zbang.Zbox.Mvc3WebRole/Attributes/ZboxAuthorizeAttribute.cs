using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.Mvc3WebRole.Attributes
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
        private readonly IFormsAuthenticationService m_FormsAuthenticationSerivce;

        public ZboxAuthorizeAttribute()
        {
            IsAuthenticationRequired = true;
            //m_ZboxReadService = DependencyResolver.Current.GetService<IZboxReadService>();
            m_FormsAuthenticationSerivce = DependencyResolver.Current.GetService<IFormsAuthenticationService>();
        }
        /// <summary>
        /// should user need to be autherise - default is true
        /// </summary>
        public bool IsAuthenticationRequired { get; set; }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var result = base.AuthorizeCore(httpContext);
            //if (!result)
            //{
            //    return false;
            //}
            //var userDetail = m_FormsAuthenticationSerivce.GetUserData();
            //if (userDetail == null)
            //{
            //    return result;
            //}
            //if (!userDetail.Verified)
            //{
            //    m_UserNotVerified = true;
            //    return false;
            //}
            return result;
            //_usersSplit = Users.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //_rolesSplit = Roles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //if (httpContext == null)
            //{
            //    throw new ArgumentNullException("httpContext");
            //}
            //if (m_ZboxReadService == null)
            //{
            //    throw new ArgumentNullException("ZboxReadService");
            //}
            //var boxuid = httpContext.Request.QueryString["BoxUid"];

            //IPrincipal user = httpContext.User;

            //if (!user.Identity.IsAuthenticated)
            //{
            //    return false;
            //}


            //if (_usersSplit.Length > 0 && !_usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase))
            //{
            //    //userValidationNotPassed = true;
            //    return false;
            //}

            //if (_rolesSplit.Length > 0 && !_rolesSplit.Any(user.IsInRole))
            //{
            //    rolesValidationNotPassed = true;
            //    return false;
            //}

            //return true;

        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            //if (m_UserNotVerified && !filterContext.HttpContext.Request.IsAjaxRequest())
            //{
            //    RedirectToVerifyAccount(filterContext);
            //    return;
            //}

            if (IsAuthenticationRequired)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    //401 asp.net get this status code and redirect to login page                
                    filterContext.Result = new HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                }
                else
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
            }
        }

        private void RedirectToVerifyAccount(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                              new RouteValueDictionary 
                               {
                                   { "action", "VerifyAccount" },
                                   { "controller", "Account" }
                               });
        }
    }
}