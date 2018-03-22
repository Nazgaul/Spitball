using System.Threading.Tasks;
using Cloudents.Api.Models;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Api.Binders
{
    public class LocationEntityBinder : IModelBinder
    {
        //private readonly ITempDataDictionaryFactory _tempDataFactory;
        private readonly IIpToLocation _ipToLocation;
        private readonly IGooglePlacesSearch _googlePlacesSearch;
        //private const string KeyName = "l1";

        public LocationEntityBinder(IIpToLocation ipToLocation,
            // ITempDataDictionaryFactory tempDataFactory,
            IGooglePlacesSearch googlePlacesSearch)
        {
            _ipToLocation = ipToLocation;
            // _tempDataFactory = tempDataFactory;
            _googlePlacesSearch = googlePlacesSearch;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // var tempData = _tempDataFactory.GetTempData(bindingContext.HttpContext);

            var latitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.{nameof(Location.Point)}.{nameof(GeographicCoordinate.Latitude)}");
            var longitudeStr = bindingContext.ValueProvider.GetValue($"{bindingContext.ModelName}.{nameof(Location.Point)}.{nameof(GeographicCoordinate.Longitude)}");
            Location locationFromTemp;// = TempDataExtensions.Get<Location>(tempData, KeyName);
            if (float.TryParse(latitudeStr.FirstValue, out var latitude)
                && float.TryParse(longitudeStr.FirstValue, out var longitude))
            {
                var point = new GeographicCoordinate
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
                //if (locationFromTemp != null)
                //{
                //    if (point == locationFromTemp.Point)
                //    {
                //        bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                //        return;
                //    }
                //}
                var resultApi = await _googlePlacesSearch.ReverseGeocodingAsync(point.ToGeoPoint(), bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
                locationFromTemp = new Location
                {
                    Address = resultApi.address,
                    Point = point,
                    Ip = bindingContext.HttpContext.Connection.GetIpAddress().ToString()
                };
                //(point,resultApi.address, bindingContext.HttpContext.Connection.GetIpAddress().ToString());
                //TempDataExtensions.Put(tempData, KeyName, locationFromTemp);
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                return;
            }

            //if (locationFromTemp != null)
            //{
            //    bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
            //    return;
            //}
            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            var retVal = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
            //TempDataExtensions.Put(tempData, KeyName, locationFromTemp);
            if (retVal == null)
            {
                return;
            }
            locationFromTemp = new Location
            {
                Address = retVal.Address,
                Point = GeographicCoordinate.FromPoint(retVal.Point),
                Ip = bindingContext.HttpContext.Connection.GetIpAddress().ToString()
            };

            bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
        }
    }
}
