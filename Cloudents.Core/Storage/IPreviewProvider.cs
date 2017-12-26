using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    public interface IPreviewProvider
    {
        Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(Uri blobUri, int indexNum,
            CancellationToken cancelToken);
    }
}
