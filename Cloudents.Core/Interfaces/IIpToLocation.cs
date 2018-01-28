using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface IIpToLocation
    {
        Task<Location> GetAsync(IPAddress address, CancellationToken token);
    }
}
