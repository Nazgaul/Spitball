using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    public interface IPreviewProvider
    {
        Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum,
            CancellationToken cancelToken);
    }
}
