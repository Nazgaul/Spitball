namespace Cloudents.Core.Storage
{
    public class FilesContainerName : IStorageContainer
    {
        public string Name => "zboxfiles";
        public string RelativePath => string.Empty;
    }
}