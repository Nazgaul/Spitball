using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IFlashcardReadSearchProvider
    {
        Task<IEnumerable<SearchFlashcard>> SearchFlashcardAsync(SearchQuery query, CancellationToken cancelToken);
    }
}