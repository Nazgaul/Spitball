using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class HomePageDataDto
    {
        public int DocumentCount { get; set; }
        public int StudentsCount { get; set; }
        public int QuizzesCount { get; set; }

        public IEnumerable<RecommendBoxDto> Boxes { get; set; }
    }

}
