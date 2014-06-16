using System;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.File
{
    public abstract class ContentProcessor
    {
        public abstract Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum);
        public abstract bool CanProcessFile(Uri blobName);
    }
}
