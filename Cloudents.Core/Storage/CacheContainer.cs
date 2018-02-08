namespace Cloudents.Core.Storage
{
    public class CacheContainer : IStorageContainer
    {
        public string Name => "zboxcahce";
        public string RelativePath => string.Empty;
    }

    public class SpitballContainer : IStorageContainer
    {
        public string Name => "spitball";
        public string RelativePath => string.Empty;
    }
}