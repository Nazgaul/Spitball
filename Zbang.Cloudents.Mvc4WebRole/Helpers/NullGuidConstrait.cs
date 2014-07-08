using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Zbang.Cloudents.Mvc4WebRole.Helpers
{
    public class NullGuidConstrait : IRouteConstraint

    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object obj;
            if (!values.TryGetValue(parameterName, out obj))
            {
                return false;
            }
            if (obj is Guid)
            {
                return true;
            }
            string input = Convert.ToString(obj, CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }
            Guid guid;
            return Guid.TryParse(input, out guid);
        }
    }
}