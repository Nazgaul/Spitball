using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentSearch
    {
        Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken);
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