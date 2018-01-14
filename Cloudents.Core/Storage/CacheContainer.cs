namespace Cloudents.Core.Storage
{
    public class CacheContainer : IStorageContainer
    {
        public string Name => "zboxcahce";
        public string RelativePath => string.Empty;
    }


    public class FilesContainerName : IStorageContainer
    {
        public string Name => "zboxfiles";
        public string RelativePath => string.Empty;
    }
}