using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class FlashcardUserDto
    {
        public IEnumerable<int> Pins { get; set; }
        public bool Like { get; set; }
    }
}
