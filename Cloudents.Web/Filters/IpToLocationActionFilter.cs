using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public class IpToLocationActionFilter : ActionFilterAttribute
    {
        private readonly string m_GeoLocationArgumentName;
        private readonly IIpToLocation m_IpToLocation;

        public IpToLocationActionFilter(string geoLocationArgumentName, IIpToLocation ipToLocation)
        {
            m_GeoLocationArgumentName = geoLocationArgumentName;
            m_IpToLocation = ipToLocation;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue(m_GeoLocationArgumentName, out var result))
            {
                await next().ConfigureAwait(false);
                return;
            }
            
            var ip = context.HttpContext.Connection.RemoteIpAddress;
            var ipV4 = ip.MapToIPv4();
            if (context.HttpContext.Connection.IsLocal())
            {
                ipV4 = IPAddress.Parse("72.229.28.185");
            }

            var ipDto = await m_IpToLocation.GetAsync(ipV4, default).ConfigureAwait(false);
            context.ActionArguments[m_GeoLocationArgumentName] = new GeoPoint
            {
                Latitude = ipDto.Latitude,
                Longitude = ipDto.Longitude
            };

            await next().ConfigureAwait(false);
            //await next().ConfigureAwait(false);
        }
    }
}
