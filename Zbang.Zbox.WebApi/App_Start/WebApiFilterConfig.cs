using System.Web.Http.Filters;
using Zbang.Zbox.WebApi.Helpers;

namespace Zbang.Zbox.WebApi
{
    public class WebApiFilterConfig
    {
        public static void RegisterGlobalFilters(HttpFilterCollection filters)
        {
            filters.Add(new GeneralExceptionFilterAttribute());
            filters.Add(new BoxAccessDeniedExceptionFilterAttribute());

        }
    }
}