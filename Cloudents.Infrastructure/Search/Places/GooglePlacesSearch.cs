using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Search.Places
{
    [UsedImplicitly]
    public class GooglePlacesSearch : IGooglePlacesSearch
    {
        private const string Key = "AIzaSyAoFR5uWJy1cf76q-J46EoEbFVZCaLk93w";//"AIzaSyAhNIR9O5bBnPZoB0lm5qRNeNN6EzjTTBg";
        //https://developers.google.com/places/web-service/search
        private readonly IRestClient _restClient;

        public GooglePlacesSearch(IRestClient restClient)
        {
            _restClient = restClient;
        }

        //[Cache(TimeConst.Month, "address", true)]
        //public async Task<(Address address, GeoPoint point)> GeoCodingByAddressAsync(string address, CancellationToken token)
        //{
        //    var nvc = new NameValueCollection
        //    {
        //        ["address"] = address,
        //        ["key"] = Key,
        //    };

        //    var str = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token);
        //    var result = SerializeResult(str);
        //    return _mapper.Map<GoogleGeoCodeDto, (Address address, GeoPoint point)>(result);
        //}

        private static GoogleGeoCodeDto SerializeResult(string str)
        {
            return JsonConvert.DeserializeObject<GoogleGeoCodeDto>(str,
                 new JsonSerializerSettings
                 {
                     ContractResolver = new UnderscorePropertyNamesContractResolver()
                 });
        }

        //[Cache(TimeConst.Year, "zip", true)]
        //public async Task<(Address address, GeoPoint point)> GeoCodingByZipAsync(string zip, CancellationToken token)
        //{
        //    var nvc = new NameValueCollection
        //    {
        //        ["components"] = "postal_code:" + zip,
        //        ["key"] = Key,
        //    };

        //    var str = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token);
        //    var source = SerializeResult(str);

        //    if (!string.Equals(source.Status, "ok", StringComparison.OrdinalIgnoreCase))
        //        return (null, null);
        //    var result = source.Results[0];

        //    var point = new GeoPoint(result.Geometry.Location.Lng, result.Geometry.Location.Lat);

        //    var city = result.AddressComponents
        //        ?.FirstOrDefault(w => w.Types.Contains("locality", StringComparer.InvariantCultureIgnoreCase))
        //        ?.ShortName;
        //    var regionCode = result.AddressComponents?.FirstOrDefault(w =>
        //            w.Types.Contains("administrative_area_level_1", StringComparer.InvariantCultureIgnoreCase))
        //        ?.ShortName;
        //    var countryCode = result.AddressComponents
        //        ?.FirstOrDefault(w => w.Types.Contains("country", StringComparer.InvariantCultureIgnoreCase))
        //        ?.ShortName;
        //    var address = new Address(city, regionCode, countryCode);
        //    return (address, point);

        //}

        [Cache(TimeConst.Year, "reverse-location", true)]
        public async Task<(Address address, GeoPoint point)> ReverseGeocodingAsync(GeoPoint point, CancellationToken token)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));
            var nvc = new NameValueCollection
            {
                ["latlng"] = $"{point.Latitude},{point.Longitude}",
                ["key"] = Key
            };

            var str = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token);

            var source = SerializeResult(str);
            if (!string.Equals(source.Status, "ok", StringComparison.OrdinalIgnoreCase))
                return (null, null);
            var result = source.Results[0];

            var point2 = new GeoPoint(result.Geometry.Location.Lng, result.Geometry.Location.Lat);

            var city = result.AddressComponents
                ?.FirstOrDefault(w => w.Types.Contains("locality", StringComparer.InvariantCultureIgnoreCase))
                ?.ShortName;
            var regionCode = result.AddressComponents?.FirstOrDefault(w =>
                    w.Types.Contains("administrative_area_level_1", StringComparer.InvariantCultureIgnoreCase))
                ?.ShortName;
            var countryCode = result.AddressComponents
                ?.FirstOrDefault(w => w.Types.Contains("country", StringComparer.InvariantCultureIgnoreCase))
                ?.ShortName;
            var address = new Address(city, regionCode, countryCode);
            return (address, point2);
        }
    }
}