using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public class IpToLocation : IIpToLocation
    {
        private readonly IRestClient _restClient;

        public IpToLocation(IRestClient restClient)
        {
            _restClient = restClient;
        }

        [Cache(TimeConst.Year, nameof(IpToLocation), true)]
        public async Task<Location> GetAsync(IPAddress ipAddress, CancellationToken token)
        {
            var uri = new Uri($"http://api.ipstack.com/{ipAddress}?access_key=0b561be1266ad6b1d01f2daedc4703cd");
            var ipDto = await _restClient.GetAsync<IpDto>(uri, null, token).ConfigureAwait(false);
            if (ipDto == null)
            {
                return null;
            }
            var point = new GeoPoint(ipDto.Longitude, ipDto.Latitude);
            var address = new Address(ipDto.City, ipDto.RegionCode, ipDto.CountryCode);
            return new Location(point, address, ipAddress.ToString());
            //var uri = new Uri($"http://freegeoip.net/json/{ipAddress}");
            //var ipDto = await _restClient.GetAsync<IpDto>(uri, null, token).ConfigureAwait(false);
            //if (ipDto == null)
            //{
            //    return null;
            //}
            //var point = new GeoPoint(ipDto.Longitude, ipDto.Latitude);
            //var address = new Address(ipDto.City, ipDto.RegionCode, ipDto.CountryCode);
            //return new Location(point, address, ipAddress.ToString());
        }

        public class IpDto
        {
            public string Ip { get; set; }
            public string Type { get; set; }
            [JsonProperty("continent_code")]
            public string ContinentCode { get; set; }
            [JsonProperty("continent_name")]
            public string ContinentName { get; set; }
            [JsonProperty("country_code")]
            public string CountryCode { get; set; }
            [JsonProperty("country_name")]
            public string CountryName { get; set; }
            public string RegionCode { get; set; }
            public string RegionName { get; set; }
            public string City { get; set; }
            public object Zip { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public IpLocation Location { get; set; }
        }

        public class IpLocation
        {
            [JsonProperty("geoname_id")]
            public int GeonameId { get; set; }
            public string Capital { get; set; }
            public IpLanguage[] Languages { get; set; }
            [JsonProperty("country_flag")]
            public string CountryFlag { get; set; }
            [JsonProperty("country_flag_emoji")]
            public string CountryFlagEmoji { get; set; }
            public string CountryFlagEmojiUnicode { get; set; }
            [JsonProperty("calling_code")]
            public string CallingCode { get; set; }
            [JsonProperty("is_eu")]
            public bool IsEu { get; set; }
        }

        public class IpLanguage
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Native { get; set; }
        }
    }
}