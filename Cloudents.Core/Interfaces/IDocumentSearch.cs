using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Query;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentSearch
    {
        Task<IList<DocumentFeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken cancelToken);
    }

    public interface IWebDocumentSearch
    {
        Task<ResultWithFacetDto<SearchResult>> SearchWithUniversityAndCoursesAsync(SearchQuery model,
             CancellationToken token);
    }

    public interface IWebFlashcardSearch
    {
        Task<ResultWithFacetDto<SearchResult>> SearchWithUniversityAndCoursesAsync(SearchQuery model,
             CancellationToken token);
    }
}