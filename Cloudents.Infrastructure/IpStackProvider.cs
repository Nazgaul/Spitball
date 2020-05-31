using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure
{
    public class IpStackProvider : IIpToLocation
    {
        private readonly HttpClient _httpClient;

        public IpStackProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Location?> GetLocationAsync(string ipAddress, CancellationToken token)
        {

            var uri = new Uri($"http://api.ipstack.com/{ipAddress}?access_key=0b561be1266ad6b1d01f2daedc4703cd");
            var ipDto = await _httpClient.GetFromJsonAsync<IpDto>(uri, token);
            if (ipDto == null)
            {
                return null;
            }
            return new Location(ipDto.CountryCode, ipDto.Location?.CallingCode);
        }

        public class IpDto
        {

            [JsonPropertyName("country_code")]
            public string CountryCode { get; set; }
            // public string RegionCode { get; set; }
            //public string City { get; set; }
            //public float? Latitude { get; set; }
            //public float? Longitude { get; set; }
            public IpLocation Location { get; set; }
        }

        public class IpLocation
        {

            [JsonPropertyName("calling_code")]
            public string CallingCode { get; set; }

        }
    }
}