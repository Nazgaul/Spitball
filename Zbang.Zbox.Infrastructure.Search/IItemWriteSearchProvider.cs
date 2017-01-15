using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IItemWriteSearchProvider
    {
        Task UpdateDataAsync(ItemSearchDto itemToUpload, IEnumerable<long> itemToDelete, CancellationToken token);
    }




}