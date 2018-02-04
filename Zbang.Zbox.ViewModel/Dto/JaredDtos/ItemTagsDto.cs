using System;
using System.Collections.Generic;
using System.Linq;

namespace Zbang.Zbox.ViewModel.Dto.JaredDtos
{
    public class ItemTagsDto
    {
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public string BoxName { get; set; }
        public long BoxId { get; set; }
        public string Department { get; set; }
        public string Blob { get; set; }
        public int DocType { get; set; }
        public string University { get; set; }
        //public string DocType
        public IEnumerable<string> Tags { get; set; }
    }

   
}
