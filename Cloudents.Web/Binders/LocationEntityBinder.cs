using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using TempDataExtensions = Cloudents.Web.Extensions.TempDataExtensions;

namespace Cloudents.Web.Binders
{
    public class LocationEntityBinder : IModelBinder
    {
        private readonly ITempDataDictionaryFactory _tempDataFactory;
        private readonly IIpToLocation _ipToLocation;
        private readonly IGooglePlacesSearch _googlePlacesSearch;
        private const string KeyName = "l1";

        public LocationEntityBinder(IIpToLocation ipToLocation, ITempDataDictionaryFactory tempDataFactory,
            IGooglePlacesSearch googlePlacesSearch)
        {
            _ipToLocation = ipToLocation;
            _tempDataFactory = tempDataFactory;
            _googlePlacesSearch = googlePlacesSearch;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var tempData = _tempDataFactory.GetTempData(bindingContext.HttpContext);

            var latitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.point.latitude");
            var longitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.point.longitude");
            var locationFromTemp = TempDataExtensions.Get<Location>(tempData, KeyName);
            if (float.TryParse(latitudeStr.FirstValue, out var latitude)
                && float.TryParse(longitudeStr.FirstValue, out var longitude))
            {
                var point = new GeoPoint(longitude, latitude);

                if (locationFromTemp != null)
                {
                    if (point == locationFromTemp.Point)
                    {
                        bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                        return;
                    }
                }
                var resultApi = await _googlePlacesSearch.ReverseGeocodingAsync(point, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
                locationFromTemp = new Location(point,resultApi.address, bindingContext.HttpContext.Connection.GetIpAddress().ToString());
                TempDataExtensions.Put(tempData, KeyName, locationFromTemp);
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
            }

            if (locationFromTemp != null)
            {
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                return;
            }
            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            locationFromTemp = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
            TempDataExtensions.Put(tempData, KeyName, locationFromTemp);
            bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
        }
    }
}
