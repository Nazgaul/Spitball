using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            var locationFromTemp = tempData.Get<LocationQuery>(KeyName);
            if (float.TryParse(latitudeStr.FirstValue, out var latitude)
                && float.TryParse(longitudeStr.FirstValue, out var longitude))
            {
                var point = new GeographicCoordinate
                {
                    Latitude = latitude,
                    Longitude = longitude
                };

                if (locationFromTemp != null)
                {
                    if (point == locationFromTemp.Point)
                    {
                        bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                        return;
                    }
                }
                var resultApi = await _googlePlacesSearch.ReverseGeocodingAsync(point.ToGeoPoint(), bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
                locationFromTemp = new LocationQuery
                {
                    Address = resultApi.address,
                    Point = point,
                    Ip = bindingContext.HttpContext.Connection.GetIpAddress().ToString()
                };
                tempData.Put(KeyName, locationFromTemp);
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                return;
            }

            if (locationFromTemp != null)
            {
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                return;
            }
            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            var retVal = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
            if (retVal == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
                
            }
            locationFromTemp = new LocationQuery
            {
                Address = retVal.Address,
                Point = GeographicCoordinate.FromPoint(retVal.Point),
                Ip = bindingContext.HttpContext.Connection.GetIpAddress().ToString()
            };

            tempData.Put(KeyName, locationFromTemp);
            bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
        }
    }
}
