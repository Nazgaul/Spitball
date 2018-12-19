using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Application.Interfaces
{
    public interface IGoogleAuth
    {
        [ItemCanBeNull]
        Task<ExternalAuthDto> LogInAsync(string token, CancellationToken cancellationToken);
    }
}