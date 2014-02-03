using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class AjaxCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Time in minutes
        /// </summary>
        public int TimeToCache { get; set; }
        public AjaxCacheAttribute()
            : this(TimeConsts.Minute * 5)
        {

        }
        public AjaxCacheAttribute(int timeToCache)
        {
            TimeToCache = timeToCache;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var headerSend = filterContext.HttpContext.Items[HTTPItemConsts.HeaderSend];
                if (headerSend != null && (bool)headerSend)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
                try
                {
                    filterContext.HttpContext.Response.Headers["Cd-Cache"] = (1000 * TimeToCache).ToString();
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("Ajax cache attribute trying to send header", ex);

                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}