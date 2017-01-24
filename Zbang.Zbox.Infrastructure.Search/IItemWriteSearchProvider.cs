using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IItemWriteSearchProvider
    {
        Task UpdateDataAsync(DocumentSearchDto itemToUpload, IEnumerable<long> itemToDelete, CancellationToken token);
    }

    public interface IContentWriteSearchProvider
    {
        Task UpdateDataAsync(ItemSearchDto itemToUpload, IEnumerable<ItemToDeleteSearchDto> itemToDelete, CancellationToken token);
    }




}