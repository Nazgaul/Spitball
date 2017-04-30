﻿using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class JaredDto
    {
        //public IEnumerable<UniversityDto> Universities { get; set; }
        public Dictionary<string, IEnumerable<string>> Terms { get; set; }
        public Dictionary<CategoryTextType, string> ActionsText { get; set; }
        public IEnumerable<BoxDto> UserBoxes { get; set; }
        public UniversityEntityDto University { get; set; }
    }

    public class UniversityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Short { get; set; }
    }
}
