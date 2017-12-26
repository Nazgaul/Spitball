namespace Cloudents.Core.Storage
{
    public class PreviewContainer : IStorageContainer
    {
        public string Name => "preview";
        public string RelativePath => string.Empty;
    }

    public class CacheContainer : IStorageContainer
    {
        public string Name => "zboxcahce";
        public string RelativePath => string.Empty;
    }
}
