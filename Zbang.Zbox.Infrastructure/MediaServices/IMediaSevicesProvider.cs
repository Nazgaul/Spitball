using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.MediaServices
{
    public interface IMediaSevicesProvider
    {

        Task<string> EncodeVideo(Uri blobUrl, CancellationToken cancelToken);
        //string EncodeToHtml5(string encodeAssetId);
        //string GetSmoothStreaming(string streamingAssetId);
    }
}
