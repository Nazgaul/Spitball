using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Mobile.Extensions;

namespace Cloudents.Mobile.Filters
{
    internal class GeoPointEntityBinder : IModelBinder
    {
        private readonly IIpToLocation _ipToLocation;


        public GeoPointEntityBinder(IIpToLocation ipToLocation)
        {
            _ipToLocation = ipToLocation;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(GeoPoint))
            {
                return false;
            }

            var latitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.latitude");
            var longitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.longitude");
            if (double.TryParse(latitudeStr?.RawValue?.ToString(), out var latitude)
                && double.TryParse(longitudeStr?.RawValue?.ToString(), out var longitude))
            {
                var point = new GeoPoint
                {
                    Longitude = longitude,
                    Latitude = latitude
                };
                bindingContext.Model = point;
                return true;
            }

            var ipV4 = actionContext.Request.GetClientIp();
            bindingContext.Model = _ipToLocation.GetAsync(ipV4, default).Result;
            return true;
        }
    }
}