using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Application.Query;
using Cloudents.Application.Request;

namespace Cloudents.Application.Interfaces
{
    public interface IDocumentSearch
    {
        //Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken);


        //Task<string> ItemMetaContentAsync(long itemId, CancellationToken cancelToken);

        Task<IList<DocumentFeedDto>> SearchDocumentsAsync(DocumentQuery query,
            CancellationToken cancelToken);
    }

    public interface IWebDocumentSearch
    {
        Task<ResultWithFacetDto<SearchResult>> SearchWithUniversityAndCoursesAsync(SearchQuery model,
            HighlightTextFormat format, CancellationToken token);
    }

    public interface IWebFlashcardSearch
    {
        Task<ResultWithFacetDto<SearchResult>> SearchWithUniversityAndCoursesAsync(SearchQuery model,
            HighlightTextFormat format, CancellationToken token);
    }
}