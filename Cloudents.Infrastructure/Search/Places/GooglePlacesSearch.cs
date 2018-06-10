using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;
using IMapper = AutoMapper.IMapper;

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
    }
}