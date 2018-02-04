using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cloudents.Web.Binders
{
    public class LocationEntityBinder : IModelBinder
    {
        private readonly IIpToLocation _ipToLocation;
        private readonly ITempDataDictionaryFactory _tempDataFactory;
        private readonly IGooglePlacesSearch _googlePlacesSearch;
        private readonly IStreamSerializer _serializer;
        private const string KeyName = "s-l";

        public LocationEntityBinder(IIpToLocation ipToLocation, ITempDataDictionaryFactory tempDataFactory, IGooglePlacesSearch googlePlacesSearch, IStreamSerializer serializer)
        {
            _ipToLocation = ipToLocation;
            _tempDataFactory = tempDataFactory;
            _googlePlacesSearch = googlePlacesSearch;
            _serializer = serializer;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var tempData = _tempDataFactory.GetTempData(bindingContext.HttpContext);

            var latitudeStr = bindingContext.ValueProvider.GetValue("location.latitude");
            var longitudeStr = bindingContext.ValueProvider.GetValue("location.longitude");
            var locationFromTemp = tempData.Get<Location>(KeyName, _serializer);
            if (double.TryParse(latitudeStr.FirstValue, out var latitude)
                && double.TryParse(longitudeStr.FirstValue, out var longitude))
            {
                var point = new GeoPoint
                {
                    Longitude = longitude,
                    Latitude = latitude
                };

                if (locationFromTemp != null)
                {
                    if (point == locationFromTemp.Point)
                    {
                        bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                        return;
                    }
                }
                locationFromTemp = await _googlePlacesSearch.ReverseGeocodingAsync(point, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
                tempData.Put(KeyName, locationFromTemp, _serializer);
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
            }

            if (locationFromTemp != null)
            {
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                return;
            }
            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            locationFromTemp = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
            tempData.Put(KeyName, locationFromTemp, _serializer);
            bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
        }
    }
}
