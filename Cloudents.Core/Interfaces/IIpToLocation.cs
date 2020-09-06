using Cloudents.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IIpToLocation
    {
        Task<Location?> GetLocationAsync(string ipAddress, CancellationToken token);
    }
}
