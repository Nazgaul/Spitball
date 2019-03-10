using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface IGoogleAuth
    {
        [ItemCanBeNull]
        Task<ExternalAuthDto> LogInAsync(string token, CancellationToken cancellationToken);
    }


    public interface IGoogleDocument
    {
        Task<string> CreateOnlineDocAsync(string documentName, CancellationToken token);
    }
}