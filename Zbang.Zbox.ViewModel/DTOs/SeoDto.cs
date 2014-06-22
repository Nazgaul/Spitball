using System.Collections.Generic;
using Zbang.Zbox.ViewModel.DTOs.BoxDtos;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;

namespace Zbang.Zbox.ViewModel.DTOs
{
    public class SeoDto
    {
        public IEnumerable<BoxSeoDto> Boxes { get; set; }
        public IEnumerable<BoxSeoDto> Items { get; set; }
        public IEnumerable<BoxSeoDto> Quizes { get; set; }
    }
}
