using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ICognitiveService
    {
        Task<Point?> DetectCenterFaceAsync(Stream stream, CancellationToken token);
        Task<Point?> DetectCenterFaceAsync(Stream stream) => DetectCenterFaceAsync(stream, default);
    }
}