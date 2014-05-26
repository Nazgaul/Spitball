using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs
{
    public class SeoDto
    {
        public IEnumerable<BoxDtos.BoxSeoDto> Boxes { get; set; }
        public IEnumerable<ItemDtos.ItemSeoDto> Items { get; set; }
    }
}
