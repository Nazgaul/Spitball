
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
        protected const string AzureBlobContainer = "zboxfiles";
        protected const string AzureCacheContainer = "zboxCahce";
        protected const string AzurePreviewContainer = "preview";
        // ReSharper disable once InconsistentNaming
        //public const string AzureQuizContainer = "zboxquestion";
        //public const string AzureChatContainer = "zboxchat";
        public const string AzureMailContainer = "mailcontainer";


        public abstract string Name
        {
            get;
        }

        public abstract string RelativePath { get; }
    }

    public class SpamGunContainerName : StorageContainerName
    {
        public override string Name => "spamgun";
        public override string RelativePath => string.Empty;
    }

    public class CacheContainerName : StorageContainerName, ICacheContainer
    {
        public override string Name => AzureCacheContainer;
        public override string RelativePath => "";
    }

    public class FilesContainerName : StorageContainerName
    {
        public override string Name => AzureBlobContainer;
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
}
