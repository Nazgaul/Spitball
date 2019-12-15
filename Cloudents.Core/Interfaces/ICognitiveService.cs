using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ICognitiveService
    {
        Task<Point?> DetectCenterFaceAsync(Stream stream);
    }
}