using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs
{
    public class SeoDto
    {
        public IEnumerable<BoxDtos.BoxSeoDto> Boxes { get; set; }
        public IEnumerable<ItemDtos.ItemSeoDto> Items { get; set; }
    }
}
