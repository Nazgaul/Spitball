using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Core.Interfaces
{
    public interface IVideoSearch
    {
        Task<VideoDto> SearchAsync(string term, CancellationToken token);
    }
}