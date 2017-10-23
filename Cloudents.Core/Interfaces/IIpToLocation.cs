using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IIpToLocation
    {
        Task<IpDto> GetAsync(IPAddress address, CancellationToken token);
    }
}
