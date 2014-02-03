using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
