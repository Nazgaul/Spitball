using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Query;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentSearch
    {
        Task<IEnumerable<DocumentFeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken cancelToken);
    }
  
}