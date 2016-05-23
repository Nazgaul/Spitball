
namespace Zbang.Zbox.Infrastructure.Storage
{
    public abstract class StorageContainerName
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
    }

    public class ChatContainerName : StorageContainerName
    {
        public override string Name => AzureChatContainer;
    }
    public class FilesContainerName : StorageContainerName
    {
        public override string Name => AzureBlobContainer;
    }


}
