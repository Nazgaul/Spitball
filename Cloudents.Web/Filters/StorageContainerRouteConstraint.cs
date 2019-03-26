//using System;
//using System.Linq;
//using Cloudents.Core.Storage;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing;

//namespace Cloudents.Web.Filters
//{
//    public class StorageContainerRouteConstraint : IRouteConstraint
//    {
//        public bool Match(HttpContext httpContext, IRouter route,
//            string routeKey, RouteValueDictionary values,
//            RouteDirection routeDirection)
//        {
//            if (!values.TryGetValue(routeKey, out var value)) return false;
//            if (value is StorageContainer)
//            {
//                return true;
//            }

//            var valueStr = value.ToString();
//            var fields = StorageContainer.GetAllValues();
//            return fields.Any(f => string.Equals(f.Name, valueStr, StringComparison.OrdinalIgnoreCase));

//        }
//    }


    
//}