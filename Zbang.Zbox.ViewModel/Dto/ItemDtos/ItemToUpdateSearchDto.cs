using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
   public  class ItemToUpdateSearchDto
    {
       public IEnumerable<ItemSearchDto> ItemsToUpdate { get; set; }

        public IEnumerable<long> ItemsToDelete { get; set; }
    }
}
