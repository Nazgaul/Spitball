using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IIpToLocation
    {
        [ItemCanBeNull]
        Task<Location> GetAsync(IPAddress ipAddress, CancellationToken token);
    }
}
