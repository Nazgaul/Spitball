using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.MobileApi.Binders
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

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var latitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.latitude").FirstValue ??
                              bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.point.latitude").FirstValue;
            var longitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.longitude").FirstValue ??
                               bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.point.longitude").FirstValue;
            if (float.TryParse(latitudeStr, out var latitude)
                && float.TryParse(longitudeStr, out var longitude))
            {
                var point = new GeoPoint(longitude, latitude);
                var ipResult =
                    await _googlePlacesSearch.ReverseGeocodingAsync(point, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);

                var location = new Location(point,ipResult.address, bindingContext.HttpContext.Connection.GetIpAddress().ToString());
                bindingContext.Result = ModelBindingResult.Success(location);
                return;
            }

            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            var locationIp = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted)
                .ConfigureAwait(false);
            bindingContext.Result = ModelBindingResult.Success(locationIp);
        }
    }
}
