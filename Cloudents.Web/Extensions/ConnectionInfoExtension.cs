using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Cloudents.Web.Extensions
{
    public static class ConnectionInfoExtension
    {
        public static bool IsLocal(this ConnectionInfo connection)
        {

            if (connection.RemoteIpAddress != null)
            {

                return IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }


        
    }

    public class RequiredArrayRouteConstraint : IRouteConstraint
    {


        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}
