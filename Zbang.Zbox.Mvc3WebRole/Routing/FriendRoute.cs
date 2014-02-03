using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Zbang.Zbox.Mvc3WebRole.Routing
{
    public class FriendRoute : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!httpContext.Request.Url.AbsolutePath.ToLower().Contains("friend") && routeDirection == RouteDirection.IncomingRequest)
            {
                return false;
            }
            if (routeDirection == RouteDirection.UrlGeneration)
            {
                return true;
            }
            if (values["friendUid"] == null)
                return false;
            if (string.IsNullOrWhiteSpace(values["friendUid"].ToString()))
            {
                return false;
            }

            return true;
        }
    }
}