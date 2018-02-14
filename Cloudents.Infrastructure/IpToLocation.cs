using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure
{
    public class IpToLocation : IIpToLocation
    {
        private readonly IRestClient _restClient;
        private readonly IMapper _mapper;

        public IpToLocation(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        [Cache(TimeConst.Year, nameof(IpToLocation))]
        public async Task<Location> GetAsync(IPAddress ipAddress, CancellationToken token)
        {
            var uri = new Uri($"http://freegeoip.net/json/{ipAddress}");
            var str = await _restClient.GetAsync(uri, null, token).ConfigureAwait(false);
            var ipDto = JsonConvert.DeserializeObject<IpDto>(str);

            var point = new GeoPoint(ipDto.Longitude, ipDto.Latitude);
            var address = new Address(ipDto.City, ipDto.RegionCode, ipDto.CountryCode);
            return new Location(point, address, ipAddress.ToString());
        }


        public class IpDto
        {
            /*    "ip": "72.229.28.185",
        "country_code": "US",
        "country_name": "United States",
        "region_code": "NY",
        "region_name": "New York",
        "city": "New York",
        "zip_code": "10036",
        "time_zone": "America/New_York",
        "latitude": 40.7605,
        "longitude": -73.9933,
        "metro_code": 501*/
            [JsonProperty("ip")]
            public string Ip { get; set; }
            [JsonProperty("country_code")]
            public string CountryCode { get; set; }
            [JsonProperty("country_name")]
            public string CountryName { get; set; }
            [JsonProperty("region_code")]
            public string RegionCode { get; set; }
            [JsonProperty("region_name")]
            public string RegionName { get; set; }
            [JsonProperty("city")]
            public string City { get; set; }
            [JsonProperty("zip_code")]
            public string ZipCode { get; set; }
            [JsonProperty("time_zone")]
            public string TimeZone { get; set; }
            [JsonProperty("latitude")]
            public double Latitude { get; set; }
            [JsonProperty("longitude")]
            public double Longitude { get; set; }
            [JsonProperty("metro_code")]
            public string MetroCode { get; set; }

            //public Location ConvertToPoint()
            //{
            //    return new Location(Longitude, Latitude, $"{City},{RegionCode}");
            //}
        }
    }
}