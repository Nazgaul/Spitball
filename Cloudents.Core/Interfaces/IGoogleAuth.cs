using Cloudents.Core.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IGoogleAuth
    {
        Task<ExternalAuthDto?> LogInAsync(string token, CancellationToken cancellationToken);
    }
}