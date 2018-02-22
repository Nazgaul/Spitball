using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Search.Places
{
    public class GooglePlacesSearch : IGooglePlacesSearch
    {
        private const string Key = "AIzaSyAoFR5uWJy1cf76q-J46EoEbFVZCaLk93w";//"AIzaSyAhNIR9O5bBnPZoB0lm5qRNeNN6EzjTTBg";
        //https://developers.google.com/places/web-service/search
        private readonly IMapper _mapper;
        private readonly IRestClient _restClient;

        public GooglePlacesSearch(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        public async Task<PlaceDto> ByIdAsync(string id, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["key"] = Key,
                ["placeid"] = id
            };

            var resultStr = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/place/details/json"), nvc, token).ConfigureAwait(false);
            var result = JObject.Parse(resultStr);
            return _mapper.Map<JObject, PlaceDto>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = Key;
            });
        }

        [Cache(TimeConst.Year, "address", true)]
        public async Task<(Address address, GeoPoint point)> GeoCodingByAddressAsync(string address, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["address"] = address,
                ["key"] = Key,
            };

            var str = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token).ConfigureAwait(false);
            var result = SerializeResult(str);
            return _mapper.Map<GoogleGeoCodeDto, (Address address, GeoPoint point)>(result);
        }

        private static GoogleGeoCodeDto SerializeResult(string str)
        {
            return JsonConvert.DeserializeObject<GoogleGeoCodeDto>(str,
                 new JsonSerializerSettings
                 {
                     ContractResolver = new UnderscorePropertyNamesContractResolver()
                 });
        }

        [Cache(TimeConst.Year, "zip", true)]
        public async Task<(Address address, GeoPoint point)> GeoCodingByZipAsync(string zip, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["components"] = "postal_code:" + zip,
                ["key"] = Key,
            };

            var str = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token).ConfigureAwait(false);
            var result = SerializeResult(str);
            return _mapper.Map<GoogleGeoCodeDto, (Address address, GeoPoint point)>(result);
        }

        [Cache(TimeConst.Year, "reverse-location", true)]
        public async Task<(Address address, GeoPoint point)> ReverseGeocodingAsync(GeoPoint point, CancellationToken token)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));
            var nvc = new NameValueCollection
            {
                ["latlng"] = $"{point.Latitude},{point.Longitude}",
                ["key"] = Key
            };

            var str = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token).ConfigureAwait(false);
            var result = SerializeResult(str);
            return _mapper.Map<GoogleGeoCodeDto, (Address address, GeoPoint point)>(result);
            //location.Point = point;
            //return location;
        }

        public async Task<PlaceDto> SearchAsync(string term, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));

            var nvc = new NameValueCollection
            {
                ["query"] = term,
                ["key"] = Key,
            };

            var resultStr = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/place/textsearch/json"), nvc, token).ConfigureAwait(false);
            var result = JObject.Parse(resultStr);
            var mapperResult = _mapper.Map<JObject, IEnumerable<PlaceDto>>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = Key;
            });
            return mapperResult.FirstOrDefault();
        }

        public async Task<(string token, IEnumerable<PlaceDto> data)> SearchNearbyAsync(IEnumerable<string> term, PlacesRequestFilter filter,
            GeoPoint location, string nextPageToken, CancellationToken token)
        {
            var nvc = BuildQuery(term, filter, location, nextPageToken);

            var resultStr = await _restClient.GetAsync(new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json"), nvc, token).ConfigureAwait(false);
            var result = JObject.Parse(resultStr);
            return _mapper.Map<JObject, (string, IEnumerable<PlaceDto>)>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = Key;
            });
        }

        private static NameValueCollection BuildQuery(IEnumerable<string> term, PlacesRequestFilter filter,
            GeoPoint location, string nextPageToken)
        {
            if (string.IsNullOrEmpty(nextPageToken))
            {
                var termStr = string.Join(" ", term ?? Enumerable.Empty<string>()) ?? string.Empty;
                if (location == null) throw new ArgumentNullException(nameof(location));
                var nvc = new NameValueCollection
                {
                    ["location"] = $"{location.Latitude} {location.Longitude}",
                    ["keyword"] = termStr,
                    ["key"] = Key,
                    ["rankby"] = "distance",
                    //["pagetoken"] = nextPageToken
                };
                if (string.IsNullOrEmpty(termStr))
                {
                    nvc.Add("type", "restaurant");
                }
                if (filter == PlacesRequestFilter.OpenNow)
                {
                    nvc.Add("opennow", true.ToString());
                }
                return nvc;
            }
            return new NameValueCollection
            {
                ["key"] = Key,
                ["pagetoken"] = nextPageToken
            };
        }
    }
}