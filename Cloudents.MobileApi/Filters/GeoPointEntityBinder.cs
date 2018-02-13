using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.MobileApi.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.MobileApi.Filters
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
            if (double.TryParse(latitudeStr.FirstValue, out var latitude)
                && double.TryParse(longitudeStr.FirstValue, out var longitude))
            {
                bindingContext.Result = ModelBindingResult.Success(new GeoPoint
                {
                    Longitude = longitude,
                    Latitude = latitude
                });
                return;
            }

            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            var location = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted);
            bindingContext.Result = ModelBindingResult.Success(location.Point);
        }
    }
}