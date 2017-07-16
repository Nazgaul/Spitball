
namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IStorageContainerName 
    {
        string Name { get; }
        string RelativePath { get; }
    }

    public interface IPreviewContainer : IStorageContainerName
    {
        
    }

    public interface ICacheContainer : IStorageContainerName
    {
        
    }

    public abstract  class StorageContainerName : IStorageContainerName
    {
        public const string AzureBlobContainer = "zboxfiles";
        public const string AzureCacheContainer = "zboxCahce";
        public const string AzureProductContainer = "zboxProductImages";
        public const string AzureProfilePicContainer = "zboxprofilepic";
        public const string AzurePreviewContainer = "preview";
        public const string AzureFaqContainer = "zboxhelp";
        public const string AzureQuizContainer = "zboxquestion";
        public const string AzureChatContainer = "zboxchat";

        public abstract string Name
        {
            get;
        }

        public abstract string RelativePath { get; }
       
    }

    public class CacheContainerName : StorageContainerName, ICacheContainer
    {
        public override string Name => AzureCacheContainer;
        public override string RelativePath => "";
    }

    public class ChatCacheContainerName : StorageContainerName, ICacheContainer
    {
        public override string Name => AzureCacheContainer;
        public override string RelativePath => "chat";
    }

    public class ChatContainerName : StorageContainerName
    {
        public override string Name => AzureChatContainer;
        public override string RelativePath => "";
    }
    public class FilesContainerName : StorageContainerName
    {
        public override string Name => AzureBlobContainer;
        public override string RelativePath => "";
    }


    public class CrawlContainerName : StorageContainerName
    {
        public override string Name => "crawl";
        public override string RelativePath => "";
    }

    public class PreviewContainerName : StorageContainerName, IPreviewContainer
    {
        public override string Name => AzurePreviewContainer;
        public override string RelativePath => "";
    }

    public class PreviewChatContainerName : StorageContainerName, IPreviewContainer
    {
        public override string Name => AzurePreviewContainer;
        public override string RelativePath => "chat";
    }

    public class FlashcardContainerName : StorageContainerName
    {
        public override string Name => AzureQuizContainer;
        public override string RelativePath => "flashcard";
    }

}
