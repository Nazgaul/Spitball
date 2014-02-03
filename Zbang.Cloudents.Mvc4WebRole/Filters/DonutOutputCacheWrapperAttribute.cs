using DevTrends.MvcDonutCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class DonutOutputCacheWrapperAttribute : DonutOutputCacheAttribute
    {
        public override void OnResultExecuted(System.Web.Mvc.ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            if (!filterContext.HttpContext.Request.Headers["Host"].ToLower().Contains("cloudents"))
            {
                var cacheManager = new OutputCacheManager();
                cacheManager.RemoveItems("Account", "Index");
            }
        }
    }
}