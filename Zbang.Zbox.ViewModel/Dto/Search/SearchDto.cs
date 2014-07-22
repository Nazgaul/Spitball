using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.Search
{
    public class SearchDto
    {
        public SearchDto()
        {
            
        }
        public IEnumerable<SearchBoxes> Boxes { get; set; }
        public IEnumerable<SearchUsers> Users { get; set; }
        public IEnumerable<SearchItems> Items { get; set; }
        public IEnumerable<SearchItems> OtherItems { get; set; }
    }
}
