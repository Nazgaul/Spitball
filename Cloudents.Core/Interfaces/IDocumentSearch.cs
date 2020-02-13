using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.Query;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentSearch
    {
        Task<IEnumerable<FeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken cancelToken);
    }

}