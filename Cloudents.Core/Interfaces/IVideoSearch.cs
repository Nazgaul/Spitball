using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IVideoSearch
    {
        Task<VideoDto> SearchAsync(IEnumerable<string> term, CancellationToken token);
    }
}