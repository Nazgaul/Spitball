using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.ViewModel.Dto.Search;

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

    public interface IContentReadSearchProvider
    {
        // Task<IEnumerable<SearchItem>> SearchAsync<T>(T query, CancellationToken cancelToken) where T : IIntent;

        Task<IEnumerable<SearchItem>> SearchAsync(BaseIntent query, CancellationToken cancelToken);
        Task<IEnumerable<SearchItem>> SearchAsync(SearchAllDocuments query, CancellationToken cancelToken);
    }
}