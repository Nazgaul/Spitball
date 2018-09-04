﻿using System;
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

        [Cache(TimeConst.Year, nameof(IpToLocation) + "2", true),Log]
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
            return new Location(point, address, ipAddress.ToString(), ipDto.Location?.CallingCode);
        }


        public class IpDto
        {
           
            [JsonProperty("country_code")]
            public string CountryCode { get; set; }
            public string RegionCode { get; set; }
            public string City { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public IpLocation Location { get; set; }
        }

        public class IpLocation
        {
          
            [JsonProperty("calling_code")]
            public string CallingCode { get; set; }
           
        }

        //public class IpLanguage
        //{
        //    public string Code { get; set; }
        //    public string Name { get; set; }
        //    public string Native { get; set; }
        //}

        //public class IpLanguage
        //{
        //    public string Code { get; set; }
        //    public string Name { get; set; }
        //    public string Native { get; set; }
        //}



        //public class IpDto
        //{
        //    /*    "ip": "72.229.28.185",
        //"country_code": "US",
        //"country_name": "United States",
        //"region_code": "NY",
        //"region_name": "New York",
        //"city": "New York",
        //"zip_code": "10036",
        //"time_zone": "America/New_York",
        //"latitude": 40.7605,
        //"longitude": -73.9933,
        //"metro_code": 501*/
        //    [JsonProperty("ip")]
        //    public string Ip { get; set; }

        //    [JsonProperty("country_code")]
        //    public string CountryCode { get; set; }

        //    [JsonProperty("country_name")]
        //    public string CountryName { get; set; }

        //    [JsonProperty("region_code")]
        //    public string RegionCode { get; set; }

        //    [JsonProperty("region_name")]
        //    public string RegionName { get; set; }

        //    [JsonProperty("city")]
        //    public string City { get; set; }

        //    [JsonProperty("zip_code")]
        //    public string ZipCode { get; set; }

        //    [JsonProperty("time_zone")]
        //    public string TimeZone { get; set; }

        //    [JsonProperty("latitude")]
        //    public float Latitude { get; set; }

        //    [JsonProperty("longitude")]
        //    public float Longitude { get; set; }

        //    [JsonProperty("metro_code")]
        //    public string MetroCode { get; set; }
        //}
    }
}