using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class SeoDto
    {
        public IEnumerable<string> Boxes { get; set; }
        public IEnumerable<string> Items { get; set; }
        public IEnumerable<string> Quizes { get; set; }
    }
}
