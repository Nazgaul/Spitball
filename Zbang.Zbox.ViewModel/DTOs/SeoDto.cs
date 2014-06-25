using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs
{
    public class SeoDto
    {
        public IEnumerable<string> Boxes { get; set; }
        public IEnumerable<string> Items { get; set; }
        public IEnumerable<string> Quizes { get; set; }
    }
}
