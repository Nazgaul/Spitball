using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Security.Principal;
using System.Threading;

namespace Zbang.Zbox.MvcWebRole.Controllers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeFlashAttribute: AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {            
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            //Get Cookie from request
            string flashCookie = HttpContext.Current.Request.Form["flashCookie"];
         
            //If Cookie found
            if (flashCookie != null)
            {
                //Get Ticket
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(flashCookie);

                if (ticket != null)
                {
             
                    var identity = new FormsIdentity(ticket);
                    
                    if (identity.IsAuthenticated)
                    {
                        SetCachePolicy(filterContext);
             
                        filterContext.RequestContext.HttpContext.User = new GenericPrincipal(identity, null);
                        Thread.CurrentPrincipal = filterContext.RequestContext.HttpContext.User;
                    }
                    else
                    {                        
                        filterContext.Result = new JsonResult() { Data = new { success = false, data = "Unauthorized" } };
                    }
                }
            }            
        }

        protected void SetCachePolicy(AuthorizationContext filterContext)
        {            
            HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
            cachePolicy.SetProxyMaxAge(new TimeSpan(0));
            cachePolicy.AddValidationCallback(CacheValidationHandler, null);
        }

        public void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }        
    }
}