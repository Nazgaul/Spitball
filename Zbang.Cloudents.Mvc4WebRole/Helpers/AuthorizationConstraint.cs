using System.Web;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class AuthorizationConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            if (httpContext.User == null)
            {
                return false;
            }
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}