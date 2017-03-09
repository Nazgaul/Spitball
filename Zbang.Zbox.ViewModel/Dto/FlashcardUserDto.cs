using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class FlashcardUserDto
    {
        public IEnumerable<int> Pins { get; set; }
        public Guid? Like { get; set; }

        public string OwnerName { get; set; }

        public bool Publish { get; set; }

        //public UniversityData UniversityData { get; set; }

        
    }
    //public class UniversityData
    //{
    //    public string UniversityName { get; set; }
    //    public string BtnColor { get; set; }
    //    public string BtnFontColor { get; set; }
    //}
}
