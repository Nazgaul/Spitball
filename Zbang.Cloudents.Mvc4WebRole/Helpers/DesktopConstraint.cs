using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class DesktopConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return !DisplayConfig.CheckIfMobileView(httpContext) && !httpContext.Request.IsAjaxRequest();
        }
    }


   

}