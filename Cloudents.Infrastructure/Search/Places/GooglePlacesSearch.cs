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

            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/place/details/json"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, PlaceDto>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = Key;
            });
        }

        [Cache(TimeConst.Year, "address")]
        public async Task<Location> GeoCodingByAddressAsync(string address, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["address"] = address,
                ["key"] = Key,
            };

            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, Location>(result);
        }


        [Cache(TimeConst.Year, "zip")]
        public async Task<Location> GeoCodingByZipAsync(string zip, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["components"] = "postal_code:" + zip,
                ["key"] = Key,
            };

            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, Location>(result);
        }

        public async Task<AddressDto> ReverseGeocodingAsync(Location point, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["latlng"] = $"{point.Latitude},{point.Longitude}",
                ["key"] = Key,
                ["result_type"] = "administrative_area_level_1|locality"
            };

            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, AddressDto>(result);
        }

        public async Task<PlaceDto> SearchAsync(string term, CancellationToken token)
        {
            if (term == null) throw new ArgumentNullException(nameof(term));

            var nvc = new NameValueCollection
            {
                ["query"] = term,
                ["key"] = Key,
            };

            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/place/textsearch/json"), nvc, token).ConfigureAwait(false);
            var mapperResult = _mapper.Map<JObject, IEnumerable<PlaceDto>>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = Key;
            });
            return mapperResult.FirstOrDefault();
        }

        public async Task<(string token, IEnumerable<PlaceDto> data)> SearchNearbyAsync(IEnumerable<string> term, PlacesRequestFilter filter,
            Location location, string nextPageToken, CancellationToken token)
        {
            var nvc = BuildQuery(term, filter, location, nextPageToken);

            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, (string, IEnumerable<PlaceDto>)>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = Key;
            });
        }

        private static NameValueCollection BuildQuery(IEnumerable<string> term, PlacesRequestFilter filter,
            Location location, string nextPageToken)
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