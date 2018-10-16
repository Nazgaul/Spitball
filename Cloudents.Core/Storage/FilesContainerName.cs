namespace Cloudents.Core.Storage
{
    public class OldSbFilesContainerName : IStorageContainer
    {
        public StorageContainer Container => StorageContainer.OldDocuments;
    }
}