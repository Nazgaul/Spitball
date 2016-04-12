using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;

namespace Zbang.Cloudents.Mvc4WebRole
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ZboxHandleErrorAttribute());
            //filters.Add(new RedirectToWWW());
            filters.Add(new RequireHttpsAttribute());
            filters.Add(new NoCacheAjaxFilterAttribute());
            filters.Add(new ETagAttribute());
            filters.Add(new StackExchange.Profiling.Mvc.ProfilingActionFilter());

        }
    }


    //public class RequireHttpsWrapperAttribute : RequireHttpsAttribute
    //{
    //    public override void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        //if (filterContext.HttpContext.Request.IsLocal)
    //        //    return;
    //        base.OnAuthorization(filterContext);
    //    }
    //}
}