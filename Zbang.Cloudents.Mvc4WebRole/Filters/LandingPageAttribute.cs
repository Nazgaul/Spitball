using System;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Controllers;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class LandingPageAttribute : ActionFilterAttribute
    {
        public LandingPageAttribute(IZboxReadService zboxReadService, ICookieHelper cookieHelper)
        {
            ZboxReadService = zboxReadService;
            CookieHelper = cookieHelper;
        }

        public IZboxReadService ZboxReadService { get;  }
        public ICookieHelper CookieHelper { get;  }

        //GetCountryByIpAsync
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (CookieHelper == null)
            //{
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}
            if (filterContext.Controller.GetType() == typeof(AlexController))
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            const string cookieName = "landing";
            var country = CookieHelper.ReadCookie<string>(cookieName);

            if (country == null)
            {
                var ip = filterContext.RequestContext.HttpContext.Request.UserHostAddress;
                if (filterContext.RequestContext.HttpContext.Request.IsLocal)
                {
                    ip = "72.229.28.185";
                }

                country = ZboxReadService.GetCountryByIp(Ip2Long(ip));
                CookieHelper.InjectCookie(cookieName, country);
            }
            if (country?.ToLowerInvariant() == "us")
            {
                filterContext.Result = new RedirectToRouteResult("Alex", null);
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        private static long Ip2Long(string ip)
        {
            double num = 0;
            if (string.IsNullOrEmpty(ip)) return (long)num;
            var ipBytes = ip.Split('.');
            for (var i = ipBytes.Length - 1; i >= 0; i--)
            {
                num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, 3 - i));
            }
            return (long)num;
        }
    }
}