using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Query;
using JetBrains.Annotations;
using Nager.Country;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public class IpToLocation : IIpToLocation
    {
        private readonly IRestClient _restClient;
        private readonly IQueryBus _queryBus;
        private static readonly ICountryProvider CountryProvider = new CountryProvider();

        public IpToLocation(IRestClient restClient, IQueryBus queryBus)
        {
            _restClient = restClient;
            _queryBus = queryBus;
        }

        [Cache(TimeConst.Month, nameof(IpToLocation) + "3", true),Log]
        public async Task<Location> GetAsync(IPAddress ipAddress, CancellationToken token)
        {
            var query = new CountryByIpQuery(ipAddress.ToString());
            var result = await _queryBus.QueryAsync(query, token);
            if (result != null)
            {
                var country = CountryProvider.GetCountry(result);
                return new Location(result,country.CallingCodes.First());
            }

            var uri = new Uri($"http://api.ipstack.com/{ipAddress}?access_key=0b561be1266ad6b1d01f2daedc4703cd");
            var ipDto = await _restClient.GetAsync<IpDto>(uri, null, token);
            if (ipDto == null)
            {
                return null;
            }

            if (ipDto.Latitude == null && ipDto.Longitude == null)
            {
                return null;
            }
            //var address = new Address(ipDto.City, ipDto.RegionCode, ipDto.CountryCode);
            return new Location(ipDto.CountryCode, ipDto.Location?.CallingCode);
        }


        public class IpDto
        {
           
            [JsonProperty("country_code")]
            public string CountryCode { get; set; }
           // public string RegionCode { get; set; }
            //public string City { get; set; }
            public float? Latitude { get; set; }
            public float? Longitude { get; set; }
            public IpLocation Location { get; set; }
        }

        public class IpLocation
        {
          
            [JsonProperty("calling_code")]
            public string CallingCode { get; set; }
           
        }

       
    }
}