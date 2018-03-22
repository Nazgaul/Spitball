using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
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

            var resultStr = await _restClient.GetAsync<GooglePlaceDto>(new Uri("https://maps.googleapis.com/maps/api/place/details/json"), nvc, token).ConfigureAwait(false);
            if (resultStr == null)
            {
                return null;
            }
            if (!string.Equals(resultStr.Status, "ok", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var photo = resultStr.Result.Photos?[0]?.PhotoReference;
            string image = null;
            if (!string.IsNullOrEmpty(photo))
            {
                image =
                    $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=150&photoreference={photo}&key={Key}";
            }
            GeoPoint location = null;
            if (resultStr.Result.Geometry.Location != null)
            {
                location = new GeoPoint(resultStr.Result.Geometry.Location.Lng,
                    resultStr.Result.Geometry.Location.Lat);
            }
            return new PlaceDto
            {
                Address = resultStr.Result.Vicinity,
                Image = image,
                Location = location,
                Name = resultStr.Result.Name,
                Open = resultStr.Result.OpeningHours?.OpenNow ?? false,
                Rating = resultStr.Result.Rating,
                PlaceId = resultStr.Result.PlaceId
            };
            //var result = JObject.Parse(resultStr);
            //return _mapper.Map<JObject, PlaceDto>(result, opt =>
            //{
            //    opt.Items["width"] = 150;
            //    opt.Items["key"] = Key;
            //});
        }

        [Cache(TimeConst.Month, "address", true)]
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
        }

        public async Task<PlacesNearbyDto> SearchNearbyAsync(IEnumerable<string> term, PlacesRequestFilter filter,
            GeoPoint location, string nextPageToken, CancellationToken token)
        {
            var nvc = BuildQuery(term, filter, location, nextPageToken);

            var resultStr = await _restClient.GetAsync<GooglePlacesDto>(new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json"), nvc, token).ConfigureAwait(false);
            //var result = JObject.Parse(resultStr);
            if (resultStr == null)
            {
                return null;
            }
            if (!string.Equals(resultStr.Status, "ok", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return new PlacesNearbyDto
            {
                Token = resultStr.NextPageToken,
                Data = resultStr.Results.Select(s =>
                {
                    var photo = s.Photos?[0]?.PhotoReference;
                    string image = null;
                    if (!string.IsNullOrEmpty(photo))
                    {
                        image =
                            $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=150&photoreference={photo}&key={Key}";
                    }
                    GeoPoint placeLocation = null;
                    if (s.Geometry?.Location != null)
                    {
                        placeLocation = new GeoPoint(s.Geometry.Location.Lng,
                            s.Geometry.Location.Lat);
                    }
                    return new PlaceDto
                    {
                        Address = s.Vicinity,
                        Image = image,
                        Location = placeLocation,
                        Name = s.Name,
                        Open = s.OpeningHours?.OpenNow ?? false,
                        Rating = s.Rating,
                        PlaceId = s.PlaceId
                    };
                })
            };
        }

        private static NameValueCollection BuildQuery(IEnumerable<string> term, PlacesRequestFilter filter,
         [NotNull]   GeoPoint location, 
            string nextPageToken)
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