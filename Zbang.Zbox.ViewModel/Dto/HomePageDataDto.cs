using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class HomePageDataDto
    {
        public HomePageStats HomePageStats { get; set; }
        public HomePageUniversityData HomePageUniversityData { get; set; }
        //public IEnumerable<RecommendBoxDto> Boxes { get; set; }
    }

    public class HomePageStats
    {
        public int DocumentCount { get; set; }
        public int StudentsCount { get; set; }
        public int BoxesCount { get; set; }
    }

    public class HomePageUniversityData
    {
        public string HeaderBackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string H1Text { get; set; }
        public string VideoBackgroundColor { get; set; }
        public string VideoFontColor { get; set; }
        public string SignupColor { get; set; }
    }

}
