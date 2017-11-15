using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure
{
    public class IpToLocation : IIpToLocation
    {
        private readonly IRestClient m_RestClient;
        private readonly IMapper m_Mapper;

        public IpToLocation(IRestClient restClient, IMapper mapper)
        {
            m_RestClient = restClient;
            m_Mapper = mapper;
        }

        public async Task<IpDto> GetAsync(IPAddress address, CancellationToken token)
        {
            var uri = new Uri($"http://freegeoip.net/json/{address}");
            var str = await m_RestClient.GetAsync(uri, null, token).ConfigureAwait(false);
            return m_Mapper.Map<IpDto>(str);
        }
    }
}