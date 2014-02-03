using System.Web;
using System.Web.Routing;

namespace Zbang.Zbox.Mvc3WebRole.Routing
{
    public class UserAuthorizeConstraint : IRouteConstraint
    {
        public bool Match
            (
                HttpContextBase httpContext,
                Route route,
                string parameterName,
                RouteValueDictionary values,
                RouteDirection routeDirection
            )
        {
           
            return !httpContext.User.Identity.IsAuthenticated;
        }
    }
}