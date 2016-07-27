using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
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

    public class UniversityConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
           
            if (httpContext.Request.IsAjaxRequest())
            {
                return false;
            }
            object obj;
            if (!values.TryGetValue(parameterName, out obj) || obj == null)
            {
                return false;
            }
            var str = obj.ToString().ToLower();
            

            if (str == "dashboard")
            {
                return false;
            }
            return true;
            //if (httpContext.User == null)
            //{
            //    return false;
            //}
            //return httpContext.User.Identity.IsAuthenticated;
        }
    }
}