using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class IpToLocationActionFilter : ActionFilterAttribute
    {
        private readonly string _geoLocationArgumentName;
        private readonly IIpToLocation _ipToLocation;

        private const string CookieName = "s-l";

        public IpToLocationActionFilter(string geoLocationArgumentName, IIpToLocation ipToLocation)
        {
            _geoLocationArgumentName = geoLocationArgumentName;
            _ipToLocation = ipToLocation;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cookie = context.HttpContext.Request.Cookies[CookieName];

            if (next == null) throw new ArgumentNullException(nameof(next));
            if (context.ActionArguments.TryGetValue(_geoLocationArgumentName, out var b)
                && b is GeoPoint placeLocation)
            {
                AppendCookie(context, placeLocation);
                await next().ConfigureAwait(false);
                return;
            }
            GeoPoint place = null;
            if (!string.IsNullOrEmpty(cookie))
            {
                var converter = TypeDescriptor.GetConverter(typeof(GeoPoint)); //cookie
                place = converter.ConvertFromInvariantString(cookie) as GeoPoint;
            }
            if (place != null)
            {
                context.ActionArguments[_geoLocationArgumentName] = place;
                await next().ConfigureAwait(false);
            }

            var ipV4 = context.HttpContext.Connection.GetIpAddress();

            var ipDto = await _ipToLocation.GetAsync(ipV4, context.HttpContext.RequestAborted).ConfigureAwait(false);
            place = new GeoPoint
            {
                Latitude = ipDto.Latitude,
                Longitude = ipDto.Longitude
            };
            context.ActionArguments[_geoLocationArgumentName] = place;
            AppendCookie(context, place);
            await next().ConfigureAwait(false);
        }

        private static void AppendCookie(ActionExecutingContext context, GeoPoint place)
        {
            context.HttpContext.Response.Cookies.Append(CookieName, place.ToString(),
                new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddDays(1)
                });
        }
    }
}
