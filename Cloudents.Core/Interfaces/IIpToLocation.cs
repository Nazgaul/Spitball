using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Models;
using JetBrains.Annotations;

namespace Cloudents.Application.Interfaces
{
    public interface IIpToLocation
    {
        [ItemCanBeNull]
        Task<Location> GetAsync(IPAddress ipAddress, CancellationToken token);
    }
}
