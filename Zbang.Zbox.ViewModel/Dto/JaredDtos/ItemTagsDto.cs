﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbang.Zbox.ViewModel.Dto.JaredDtos
{
    public class ItemTagsDto
    {
        public string ItemName { get; set; }
        public string BoxName { get; set; }
        public string Department { get; set; }
        public string Blob { get; set; }
        public string DocType { get; set; }
        //public string DocType
        public IEnumerable<string> Tags { get; set; }
    }
}
