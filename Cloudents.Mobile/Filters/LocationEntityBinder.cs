using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Mobile.Extensions;

namespace Cloudents.Mobile.Filters
{
    internal class LocationEntityBinder : IModelBinder
    {
        private readonly IIpToLocation _ipToLocation;
        private readonly IGooglePlacesSearch _googlePlacesSearch;

        public LocationEntityBinder(IIpToLocation ipToLocation, IGooglePlacesSearch googlePlacesSearch)
        {
            _ipToLocation = ipToLocation;
            _googlePlacesSearch = googlePlacesSearch;
        }

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(Location))
            {
                return false;
            }
            var latitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.latitude") ??
                              bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.point.latitude");
            var longitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.longitude") ??
                               bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.point.longitude");
            if (double.TryParse(latitudeStr?.RawValue?.ToString(), out var latitude)
                && double.TryParse(longitudeStr?.RawValue?.ToString(), out var longitude))
            {
                var point = new GeoPoint
                {
                    Longitude = longitude,
                    Latitude = latitude
                };
                bindingContext.Model = _googlePlacesSearch.ReverseGeocodingAsync(point, default).Result;
                return true;
            }

            var ipV4 = actionContext.Request.GetClientIp();
            bindingContext.Model = _ipToLocation.GetAsync(ipV4, default).Result;
            return true;
        }
    }
}
