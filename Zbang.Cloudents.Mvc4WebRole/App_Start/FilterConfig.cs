using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StackExchange.Profiling;
using StackExchange.Profiling.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ProfilingActionFilter(), 0);
            filters.Add(new ZboxHandleErrorAttribute());
            //filters.Add(new RedirectToWWW());
            filters.Add(new RequireHttpsAttribute());
            filters.Add(new NoCacheAjaxFilterAttribute());
            filters.Add(new ETagAttribute());


        }
        
    }
}