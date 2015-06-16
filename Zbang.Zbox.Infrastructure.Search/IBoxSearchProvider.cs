using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IBoxWriteSearchProvider2
    {
        Task<bool> UpdateData(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete);
    }

    public interface IBoxReadSearchProvider2
    {
        Task<IEnumerable<SearchBoxes>> SearchBox(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
    }
}
