using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Mobile.Filters
{
    public class RedirectToDekstopSiteAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var urlToRedirect = ConfigFetcher.Fetch("DesktopWebSite");
            var url = filterContext.HttpContext.Request.Url;
            urlToRedirect = urlToRedirect + url.PathAndQuery;

            filterContext.Result = new RedirectResult(urlToRedirect);
            return;
        }
    }
}