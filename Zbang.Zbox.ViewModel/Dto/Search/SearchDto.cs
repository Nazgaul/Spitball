using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Search
{
    public class SearchDto
    {
        public IEnumerable<SearchBoxes> Boxes { get; set; }
        public IEnumerable<SearchUsers> Users { get; set; }
        public IEnumerable<SearchItems> Items { get; set; }
        public IEnumerable<SearchItems> OtherItems { get; set; }
    }
}
