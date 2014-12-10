using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Filters;
using Zbang.Cloudents.Mvc4WebRole.Filters;

namespace Zbang.Cloudents.Mvc4WebRole
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