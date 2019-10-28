using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public interface IFileProcessorFactory
    {
        IFileProcessor GetProcessor(CloudBlockBlob blob);
    }
}