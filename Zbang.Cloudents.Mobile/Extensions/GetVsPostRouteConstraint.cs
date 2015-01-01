using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Zbang.Cloudents.Mobile.Extensions
{
    public class GetVsPostRouteConstraint : IRouteConstraint
    {
        private readonly string m_RequestType;
        public GetVsPostRouteConstraint(string requestType)
        {
            m_RequestType = requestType;
        }
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.UrlGeneration)
            {
                return true;
            }
            if (String.Equals(httpContext.Request.RequestType, m_RequestType, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}