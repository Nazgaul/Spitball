using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Cloudents.FunctionsV2.FileProcessor
{
    public interface IFileProcessor
    {
        Task ProcessFileAsync(long id, CloudBlockBlob blob, ILogger log, CancellationToken token);
    }
}