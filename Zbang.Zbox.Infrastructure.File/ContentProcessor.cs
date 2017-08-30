using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class ContentProcessor
    {
        public abstract Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken));
        public abstract bool CanProcessFile(Uri blobName);
    }
}
