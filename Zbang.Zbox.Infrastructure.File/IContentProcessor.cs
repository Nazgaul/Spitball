using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.File
{
    public interface IContentProcessor
    {
        Task<PreviewResult> ConvertFileToWebSitePreview(Uri contentUrl, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken));


        bool CanProcessFile(Uri contentUrl);

        //TODO: split to a different interface
        Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken));

        string GetDefaultThumbnailPicture();
     
    }

   
}
