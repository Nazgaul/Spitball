using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.MediaServices;

namespace Zbang.Zbox.Infrastructure.StorageApp
{
    public class MediaSevicesProvider : IMediaSevicesProvider
    {
        public System.Threading.Tasks.Task<string> EncodeVideo(Uri blobUri)
        {
            throw new NotImplementedException();
        }
    }
}
