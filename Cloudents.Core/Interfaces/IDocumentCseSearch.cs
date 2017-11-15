using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentCseSearch
    {
        Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model,
            CancellationToken token);
    }
}
