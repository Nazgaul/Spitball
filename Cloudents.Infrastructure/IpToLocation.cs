using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;

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

        public async Task<Location> GetAsync(IPAddress address, CancellationToken token)
        {
            var uri = new Uri($"http://freegeoip.net/json/{address}");
            var str = await _restClient.GetAsync(uri, null, token).ConfigureAwait(false);
            return _mapper.Map<Location>(str);
        }
    }
}