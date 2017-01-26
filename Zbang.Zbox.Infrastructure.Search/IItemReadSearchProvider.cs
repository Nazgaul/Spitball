using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Search
{


    public interface IItemReadSearchProvider
    {
        Task<IEnumerable<SearchDocument>> SearchItemAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
        Task<IEnumerable<SearchDocument>> SearchItemAsync(ViewModel.Queries.Search.SearchQueryMobile query, CancellationToken cancelToken);


        Task<IEnumerable<SearchDocument>> SearchItemAsync(ViewModel.Queries.Search.SearchItemInBox query,
            CancellationToken cancelToken);

        Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken);
    }

    public interface IContentReadSearchProvider : ISearchReadProvider
    {

    }

    public interface ISearchReadProvider
    {

        Task<SearchJaredDto> SearchAsync(KnownIntent query, SearchJared extra, CancellationToken cancelToken);
    }


}