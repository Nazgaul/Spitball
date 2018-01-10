﻿using System;
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
        private readonly IRestClient _restClient;
        private readonly IMapper _mapper;

        public IpToLocation(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        public async Task<IpDto> GetAsync(IPAddress address, CancellationToken token)
        {
            var uri = new Uri($"http://freegeoip.net/json/{address}");
            var str = await _restClient.GetAsync(uri, null, token).ConfigureAwait(false);
            return _mapper.Map<IpDto>(str);
        }
    }
}