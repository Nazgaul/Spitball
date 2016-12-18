using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class FlashcardSearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string BoxName { get; set; }


        public IEnumerable<string> FrontCards { get; set; }
        public IEnumerable<string> BackCards { get; set; }
        public IEnumerable<long> UserIds { get; set; }

        public long BoxId { get; set; }
        public long? UniversityId { get; set; }
        public string UniversityName { get; set; }

    }

    public class FlashcardToUpdateSearchDto
    {
        public IEnumerable<FlashcardSearchDto> Updates { get; set; }

        public IEnumerable<long> Deletes { get; set; }
    }
}
