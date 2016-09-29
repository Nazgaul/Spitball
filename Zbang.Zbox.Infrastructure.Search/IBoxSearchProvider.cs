using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IBoxWriteSearchProvider2
    {
        Task<bool> UpdateDataAsync(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete);
    }

    public interface IBoxReadSearchProvider2
    {
        Task<IEnumerable<SearchBoxes>> SearchBoxAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
        //Task<IEnumerable<SearchBoxes>> SearchBoxAsync(ViewModel.Queries.Search.SearchQueryMobile query, CancellationToken cancelToken);

        Task<IEnumerable<SearchBoxes>> SearchBoxWithoutHighlightWithUrlAsync(ViewModel.Queries.Search.SearchQuery query,
            CancellationToken cancelToken);
    }
}
