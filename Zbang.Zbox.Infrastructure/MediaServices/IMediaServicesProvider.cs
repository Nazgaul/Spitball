using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.MediaServices
{
    public interface IMediaServicesProvider
    {

        Task<Uri> EncodeVideoAsync(Uri blobUrl, CancellationToken cancelToken);
        //string EncodeToHtml5(string encodeAssetId);
        //string GetSmoothStreaming(string streamingAssetId);
    }
}
