using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
