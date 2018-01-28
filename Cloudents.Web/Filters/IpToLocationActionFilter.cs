using System;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cloudents.Web.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class IpToLocationActionFilter : ActionFilterAttribute
    {
        private readonly string _geoLocationArgumentName;
        private readonly IIpToLocation _ipToLocation;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        private const string CookieName = "s-l";

        public IpToLocationActionFilter(string geoLocationArgumentName, IIpToLocation ipToLocation, ITempDataDictionaryFactory tempData)
        {
            _geoLocationArgumentName = geoLocationArgumentName;
            _ipToLocation = ipToLocation;
            _tempDataFactory = tempData;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));
            var tempData = _tempDataFactory.GetTempData(context.HttpContext);
            if (tempData.Peek(CookieName) is Location place)
            {
                context.ActionArguments[_geoLocationArgumentName] = place;
                await next().ConfigureAwait(false);
                return;
            }
            if (context.ActionArguments.TryGetValue(_geoLocationArgumentName, out var b)
                && b is Location placeLocation)
            {
                tempData.Add(CookieName, placeLocation);
                await next().ConfigureAwait(false);
                return;
            }

            var ipV4 = context.HttpContext.Connection.GetIpAddress();

            place = await _ipToLocation.GetAsync(ipV4, context.HttpContext.RequestAborted).ConfigureAwait(false);

            context.ActionArguments[_geoLocationArgumentName] = place;
            tempData.Add(CookieName, place);
            await next().ConfigureAwait(false);
        }


    }
}