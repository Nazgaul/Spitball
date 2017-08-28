using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public  class ItemToUpdateSearchDto
    {
        public IEnumerable<DocumentSearchDto> ItemsToUpdate { get; set; }

        public IEnumerable<DocumentToDeleteSearchDto> ItemsToDelete { get; set; }
    }

    public abstract class ItemToDeleteSearchDto
    {
        public long Id { get; set; }
        public abstract string SearchContentId { get; }
    }

    public class DocumentToDeleteSearchDto : ItemToDeleteSearchDto
    {
        public override string SearchContentId => "item_" + Id;
    }

}
