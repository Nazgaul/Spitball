using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;

namespace Zbang.Cloudents.Mobile
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ZboxHandleErrorAttribute());
            filters.Add(new RequireHttpsAttribute());
            filters.Add(new NoCacheAjaxFilterAttribute());
            filters.Add(new ETagAttribute());
        }
    }
}