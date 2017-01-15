using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
  

    public interface IItemReadSearchProvider 
    {
        Task<IEnumerable<SearchItems>> SearchItemAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
        Task<IEnumerable<SearchItems>> SearchItemAsync(ViewModel.Queries.Search.SearchQueryMobile query, CancellationToken cancelToken);


        Task<IEnumerable<SearchItems>> SearchItemAsync(ViewModel.Queries.Search.SearchItemInBox query,
            CancellationToken cancelToken);

        Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken);
    }
}