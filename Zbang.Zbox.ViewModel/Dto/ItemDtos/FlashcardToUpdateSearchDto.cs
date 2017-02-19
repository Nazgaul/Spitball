using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class FlashcardToUpdateSearchDto
    {
        public IEnumerable<FlashcardSearchDto> Updates { get; set; }

        public IEnumerable<FlashcardToDeleteSearchDto> Deletes { get; set; }
    }
}