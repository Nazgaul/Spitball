namespace Cloudents.Core.Storage
{
    public class FilesContainerName : IStorageContainer
    {
        public StorageContainer Container => StorageContainer.Documents;
    }
}