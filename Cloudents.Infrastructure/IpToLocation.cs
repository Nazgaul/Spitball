using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Query;
using Cloudents.Query.General;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ICountryProvider = Cloudents.Core.Interfaces.ICountryProvider;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public class IpToLocation : IIpToLocation
    {
        private readonly IRestClient _restClient;
        private readonly IQueryBus _queryBus;
        private readonly ICountryProvider _countryProvider;

        public IpToLocation(IRestClient restClient, IQueryBus queryBus, ICountryProvider countryProvider)
        {
            _restClient = restClient;
            _queryBus = queryBus;
            _countryProvider = countryProvider;
        }

        [Cache(TimeConst.Month, nameof(IpToLocation) + "3", true), Log]
        public async Task<Location> GetAsync(IPAddress ipAddress, CancellationToken token)
        {
            var query = new CountryByIpQuery(ipAddress.ToString());
            var result = await _queryBus.QueryAsync(query, token);
            if (result != null)
            {
                var callingCode = _countryProvider.GetCallingCode(result);
                return new Location(result, callingCode);
            }

            var uri = new Uri($"http://api.ipstack.com/{ipAddress}?access_key=0b561be1266ad6b1d01f2daedc4703cd");
            var ipDto = await _restClient.GetAsync<IpDto>(uri, null, token);
            if (ipDto == null)
            {
                return null;
            }

            //if (ipDto.Latitude == null && ipDto.Longitude == null)
            //{
            //    return null;
            //}
            //var address = new Address(ipDto.City, ipDto.RegionCode, ipDto.CountryCode);
            return new Location(ipDto.CountryCode, ipDto.Location?.CallingCode);
        }


        public class IpDto
        {

            [JsonProperty("country_code")]
            public string CountryCode { get; set; }
            // public string RegionCode { get; set; }
            //public string City { get; set; }
            //public float? Latitude { get; set; }
            //public float? Longitude { get; set; }
            public IpLocation Location { get; set; }
        }

        public class IpLocation
        {

            [JsonProperty("calling_code")]
            public string CallingCode { get; set; }

        }


    }


    public class CountryProvider : ICountryProvider
    {
        private static readonly Nager.Country.ICountryProvider Item = new Nager.Country.CountryProvider();
        public string GetCallingCode(string countryCode)
        {
            
            var country = Item.GetCountry(countryCode);
            return country.CallingCodes.First();
        }

        public bool ValidateCountryCode(string countryCode)
        {
            var country = Item.GetCountry(countryCode);
            return country != null;
        }

        //public decimal ConvertPointsToLocalCurrency(string countryCode, decimal points)
        //{
        //    var country = Item.GetCountry(countryCode);

        //    if (country.Alpha2Code == Alpha2Code.IN)
        //    {
        //        return points;
        //    }

        //    return points / 25;
        //}

        //public string ConvertPointsToLocalCurrencyWithSymbol(string countryCode, decimal points)
        //{
        //    var country = Item.GetCountry(countryCode);
        //    if (country.Alpha2Code == Alpha2Code.IN)
        //    {
        //        var culture = CultureInfo.CurrentCulture.ChangeCultureBaseOnCountry(Alpha2Code.IN.ToString());
        //        return points.ToString("C", culture);
        //    }
        //    var culture2 = CultureInfo.CurrentCulture.ChangeCultureBaseOnCountry(Alpha2Code.IL.ToString());
        //    return (points / 25).ToString("C", culture2);
        //}

        //public string ConvertToLocalCurrencyWithSymbol(string countryCode, decimal price)
        //{
        //    var country = Item.GetCountry(countryCode);
        //    var culture = CultureInfo.CurrentCulture.ChangeCultureBaseOnCountry(country.Alpha2Code.ToString());
        //    return price.ToString("C", culture);
        //}
    }
}