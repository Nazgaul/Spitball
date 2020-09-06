namespace Cloudents.Core.Interfaces
{
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
        // public static AssetType StudyRoom = new AssetType("video-studyRoom-");

        public override string ToString()
        {
            return _prefix.ToLowerInvariant();
        }
    }
}