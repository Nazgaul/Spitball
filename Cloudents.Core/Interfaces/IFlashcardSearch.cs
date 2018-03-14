using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IFlashcardSearch
    {
        //Task<(IEnumerable<SearchResult> result, string[] facet)> SearchAsync(SearchQuery model,
        //    CancellationToken token);

        Task<ResultWithFacetDto<SearchResult>> SearchAsync(SearchQuery model, HighlightTextFormat format,
            CancellationToken token);
    }
}