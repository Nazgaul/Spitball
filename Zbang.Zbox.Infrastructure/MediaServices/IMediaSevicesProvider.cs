using System;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.MediaServices
{
    public interface IMediaSevicesProvider
    {

        Task<string> EncodeVideo(Uri blobUri);
        //string EncodeToHtml5(string encodeAssetId);
        //string GetSmoothStreaming(string streamingAssetId);
    }
}
