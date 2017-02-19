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
        public IEnumerable<long> UserIds { get; set; }

        public long BoxId { get; set; }
        public long? UniversityId { get; set; }

        public int CardsCount { get; set; }

        public override string Content => string.Join(" ", FrontCards) + string.Join(" ", BackCards);

        public string CoverText { get; set; }
        public string CoverImage { get; set; }

        public string FrontText { get; set; }
        public string FrontImage { get; set; }

        public override string[] MetaContent => new[]
        {
            FrontText ?? string.Empty,
            FrontImage ?? string.Empty,
            CoverText ?? string.Empty,
            CoverImage ?? string.Empty
        };


        public override int? ContentCount => CardsCount;
    }
}
