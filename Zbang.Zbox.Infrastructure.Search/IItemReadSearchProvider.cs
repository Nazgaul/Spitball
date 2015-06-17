using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IItemReadSearchProvider
    {
        Task<IEnumerable<SearchItems>> SearchItem(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
    }

    public interface IItemReadSearchProvider2 : IItemReadSearchProvider
    {
        
    }
}