using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Cloudents.Mvc4WebRole.Extensions
{
    public class RouteUidConstraint : IRouteConstraint
    {
        public RouteUidConstraint()
        {
           
        }
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext.Request.IsAjaxRequest() &&
                routeDirection == RouteDirection.IncomingRequest)
            {
                return false;
            }
            if (httpContext.Request.HttpMethod.ToUpper() == "POST")
            {
                return false;
            }
            var value = values["BoxUid"];

            if (value == null)
            {
                return false;
            }
            if (value.ToString().Length != 11)
            {
                return false;
            }
            return true;
        }
    }
}