using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Storage
{
    public interface IContentProcessor
    {
        Task<PreviewResult> ConvertFileToWebSitePreviewAsync(Uri contentUrl, int indexNum, CancellationToken cancelToken = default(CancellationToken));


        bool CanProcessFile(Uri contentUrl);

        //TODO: split to a different interface
        Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken));

        Task<string> ExtractContentAsync(Uri blobUri,CancellationToken cancelToken = default(CancellationToken));

    }

   
}
