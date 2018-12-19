
namespace Cloudents.Application.Storage
{
    public interface IStorageContainer
    {
        StorageContainer Container { get; }
    }

    public sealed class StorageContainer
    {

        private StorageContainer(string name)
        {
            Name = name.ToLowerInvariant();
            RelativePath = string.Empty;
        }

        private StorageContainer(string name,string relativePath)
        {
            Name = name.ToLowerInvariant();
            RelativePath = relativePath;
        }

        public string Name { get; }
        public string RelativePath { get; }

        public static readonly StorageContainer OldCacheContainer = new StorageContainer("zboxcahce");
        public static readonly StorageContainer SpitballContainer = new StorageContainer("zboxhelp", "imageProvider");
        public static readonly StorageContainer QuestionsAndAnswers = new StorageContainer("spitball-files","question");
        public static readonly StorageContainer Document = new StorageContainer("spitball-files","files");
        public static readonly StorageContainer OldDocuments = new StorageContainer("zboxFiles");
        public static readonly StorageContainer IcoFiles = new StorageContainer("zboxhelp","ico");
    }
}
