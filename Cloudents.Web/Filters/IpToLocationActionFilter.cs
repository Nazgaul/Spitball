using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    [AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false)]
    public class IpToLocationActionFilter : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("location", out var result))
            {
                return next();
            }
            if (!(result is GeoPoint location))
            {
                return next();
            }
            var ip = context.HttpContext.Connection.RemoteIpAddress;
            var ipV4 = ip.MapToIPv4();

            return next();
            //await next().ConfigureAwait(false);
        }
    }
}
