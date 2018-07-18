using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Web.Binders
{
    internal class GeoPointEntityBinder : IModelBinder
    {
        private readonly IIpToLocation _ipToLocation;

        public GeoPointEntityBinder(IIpToLocation ipToLocation)
        {
            _ipToLocation = ipToLocation;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var latitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.latitude");
            var longitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.longitude");
            if (float.TryParse(latitudeStr.FirstValue, out var latitude)
                && float.TryParse(longitudeStr.FirstValue, out var longitude))
            {
                bindingContext.Result = ModelBindingResult.Success(new GeographicCoordinate
                {
                    Latitude = latitude,
                    Longitude = longitude
                });

                return;
            }

            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            var location = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
            if (location == null)
            {
                ModelBindingResult.Failed();
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(GeographicCoordinate.FromPoint(location.Point));
        }
    }
}