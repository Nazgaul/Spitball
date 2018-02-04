using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IBoxWriteSearchProvider2
    {
        Task<bool> UpdateDataAsync(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete);
    }

}
