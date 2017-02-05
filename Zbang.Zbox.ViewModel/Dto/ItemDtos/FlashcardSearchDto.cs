using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class FlashcardSearchDto : ItemSearchDto
    {
        public override IEnumerable<ItemType> Type => new[] { ItemType.Flashcard };
        public override string SearchContentId => "flashcard_" + Id;


        public IEnumerable<string> FrontCards { get; set; }
        public IEnumerable<string> BackCards { get; set; }
        public IEnumerable<ItemSearchUsers> UserIds { get; set; }

        public long BoxId { get; set; }
        public long? UniversityId { get; set; }

        public override string Content => string.Join(" ", FrontCards) + string.Join(" ", BackCards);
    }

    public class FlashcardToUpdateSearchDto
    {
        public IEnumerable<FlashcardSearchDto> Updates { get; set; }

        public IEnumerable<FlashcardToDeleteSearchDto> Deletes { get; set; }
    }

    public class FlashcardToDeleteSearchDto : ItemToDeleteSearchDto
    {
        public override string SearchContentId => "flashcard_" + Id;
    }
}
