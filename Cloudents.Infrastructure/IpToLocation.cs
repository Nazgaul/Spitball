using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Models;
using Cloudents.Query;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using ICountryProvider = Cloudents.Core.Interfaces.ICountryProvider;

namespace Cloudents.Infrastructure
{
    public class CountryByIpQuery : IQuery<Location?>
    {
        public CountryByIpQuery(string ip)
        {
            Ip = ip;
        }

        private string Ip { get; }

        internal sealed class CountryByIpQueryQueryHandler : IQueryHandler<CountryByIpQuery, Location?>
        {
            private readonly IStatelessSession _session;
            private readonly IIpToLocation _restClient;
            private readonly ICountryProvider _countryProvider;

            public CountryByIpQueryQueryHandler(IStatelessSession session, IIpToLocation restClient, ICountryProvider countryProvider)
            {
                _restClient = restClient;
                _countryProvider = countryProvider;
                _session = session;
            }

            [Cache(TimeConst.Hour, "IpToLocation" + "3", true)]
            public async Task<Location?> GetAsync(CountryByIpQuery query, CancellationToken token2)
            {
                using var c = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var source = CancellationTokenSource.CreateLinkedTokenSource(token2, c.Token);
                try
                {
                    var newToken = source.Token;
                    var result = await _session.Query<UserLocation>()
                        .WithOptions(w =>
                        {
                            w.SetComment(nameof(CountryByIpQuery));
                            w.SetTimeout(5);

                        })
                        .Fetch(f => f.User)
                        .Where(w => w.Ip == query.Ip && w.TimeStamp.CreationTime > DateTime.UtcNow.AddDays(-30))
                        .Select(s => s.Country)
                        .FirstOrDefaultAsync(newToken);




                    if (result != null)
                    {
                        var callingCode = _countryProvider.GetCallingCode(result);
                        return new Location(result, callingCode);
                    }
                    if (newToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    return await _restClient.GetLocationAsync(query.Ip, newToken);

                }
                catch (OperationCanceledException)
                {
                    return null;
                }
            }
        }

    }

    //public class IpToLocation : IIpToLocation
    //{

    //    private readonly IQueryBus _queryBus;
    //    private readonly IStatelessSession _session;

    //    public IpToLocation(IRestClient restClient, IQueryBus queryBus, ICountryProvider countryProvider)
    //    {
    //        _restClient = restClient;
    //        _queryBus = queryBus;
    //        _countryProvider = countryProvider;
    //    }

    //    [Cache(TimeConst.Hour, nameof(IpToLocation) + "3", true)]
    //    public async Task<Location?> GetAsync(IPAddress ipAddress, CancellationToken token)
    //    {
    //        var query = new CountryByIpQuery(ipAddress.ToString());
    //        var result = await _queryBus.QueryAsync(query, token);
    //        if (result != null)
    //        {
    //            var callingCode = _countryProvider.GetCallingCode(result);
    //            return new Location(result, callingCode);
    //        }

    //        var uri = new Uri($"http://api.ipstack.com/{ipAddress}?access_key=0b561be1266ad6b1d01f2daedc4703cd");
    //        var ipDto = await _restClient.GetAsync<IpDto>(uri, null, token);
    //        if (ipDto == null)
    //        {
    //            return null;
    //        }
    //        return new Location(ipDto.CountryCode, ipDto.Location?.CallingCode);
    //    }


    //    public class IpDto
    //    {

    //        [JsonProperty("country_code")]
    //        public string CountryCode { get; set; }
    //        // public string RegionCode { get; set; }
    //        //public string City { get; set; }
    //        //public float? Latitude { get; set; }
    //        //public float? Longitude { get; set; }
    //        public IpLocation Location { get; set; }
    //    }

    //    public class IpLocation
    //    {

    //        [JsonProperty("calling_code")]
    //        public string CallingCode { get; set; }

    //    }


    //}
}