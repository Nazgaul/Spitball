using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IVideoService
    {
        Task CreateVideoPreviewJobAsync(long id, string url, CancellationToken token);
        Task<string> GetAssetContainerAsync(long id, AssetType type, CancellationToken token);

        Task DeleteImageAssetAsync(long id, CancellationToken token);
        Task<string> BuildUserStreamingLocatorAsync(long videoId,  CancellationToken token);
        Task<string> GetShortStreamingUrlAsync(long videoId, CancellationToken token);
        Task CreateShortStreamingLocator(long videoId, CancellationToken token);
    }

    public class AssetType 
    {
        private readonly string _prefix;

        private AssetType(string prefix)
        {
            _prefix = prefix;
        }

        public static AssetType Thumbnail = new AssetType("video-thumbnail-");
        public static AssetType Short = new AssetType("video-short-");
        public static AssetType Long = new AssetType("video-");

        public override string ToString()
        {
            return _prefix;
        }
    }
}