using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Search
{
    public class PlacesSearch : IPlacesSearch
    {
        //https://developers.google.com/places/web-service/search
        private readonly IMapper _mapper;
        private readonly IRestClient _restClient;

        private const string Key = "AIzaSyAoFR5uWJy1cf76q-J46EoEbFVZCaLk93w";//"AIzaSyAhNIR9O5bBnPZoB0lm5qRNeNN6EzjTTBg";

        public PlacesSearch(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        public async Task<(string token, IEnumerable<PlaceDto> data)> SearchNearbyAsync(string term, PlacesRequestFilter filter,
            GeoPoint location, string nextPageToken, CancellationToken token)
        {
            var nvc = BuildQuery(term, filter, location, nextPageToken);
            if (filter == PlacesRequestFilter.OpenNow)
            {
                nvc.Add("opennow", true.ToString());
            }
            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/place/nearbysearch/json"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, (string, IEnumerable<PlaceDto>)>(result, opt =>
            {
                opt.Items["width"] = 150;
                opt.Items["key"] = Key;
            });
        }

        public async Task<GeoPoint> GeoCodingAsync(string location, CancellationToken token)
        {
            var nvc = new NameValueCollection
            {
                ["address"] = location,
                ["key"] = Key,
            };

            var result = await _restClient.GetJsonAsync(new Uri("https://maps.googleapis.com/maps/api/geocode/json"), nvc, token).ConfigureAwait(false);
            return _mapper.Map<JObject, GeoPoint>(result);
        }

        private static NameValueCollection BuildQuery(string term, PlacesRequestFilter filter,
            GeoPoint location, string nextPageToken)
        {
            if (string.IsNullOrEmpty(nextPageToken))
            {
                if (term == null) throw new ArgumentNullException(nameof(term));
                if (location == null) throw new ArgumentNullException(nameof(location));
                return new NameValueCollection
                {
                    ["location"] = $"{location.Latitude} {location.Longitude}",
                    ["keyword"] = term,
                    ["key"] = Key,
                    ["rankby"] = "distance",
                    ["pagetoken"] = nextPageToken
                };
            }
            return new NameValueCollection
            {
                ["key"] = Key,
                ["pagetoken"] = nextPageToken
            };
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
    }
}
