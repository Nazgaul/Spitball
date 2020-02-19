using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IVideoService
    {
        Task CreateVideoPreviewJobAsync(long id, string url, CancellationToken token);
        /// <summary>
        /// Return the container of the asset
        /// </summary>
        /// <param name="id">the id of the video</param>
        /// <param name="type">the type of the asset</param>
        /// <param name="token"></param>
        /// <returns>the container name null if the asset doesn't exists</returns>
        Task<string> GetAssetContainerAsync(long id, AssetType type, CancellationToken token);

        Task DeleteImageAssetAsync(long id, CancellationToken token);

        Task DeleteAssetAsync(string assetName, CancellationToken token);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="token"></param>
        /// <returns>return url for stream - null if none exists</returns>
        Task<string> GetShortStreamingUrlAsync(long videoId, CancellationToken token);
        Task CreateShortStreamingLocator(long videoId, CancellationToken token);
        Task CreatePreviewJobAsync(long id, string url, System.TimeSpan videoLength, CancellationToken token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns>return url for stream - null if none exists</returns>
        Task<string> BuildUserStreamingLocatorAsync(long videoId, long userId, CancellationToken token);
        Task CreateAudioPreviewJobAsync(long id, string url, CancellationToken token);
        Task CreateStudyRoomSessionEncoding(string id, string url, CancellationToken token);
        Task RemoveUnusedStreamingLocatorAsync(CancellationToken token);
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
        public static AssetType StudyRoom = new AssetType("video-studyRoom-");

        public override string ToString()
        {
            return _prefix.ToLowerInvariant();
        }
    }
}