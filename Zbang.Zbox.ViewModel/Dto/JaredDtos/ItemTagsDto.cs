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
        public string DocType { get; set; }
        public string University { get; set; }
        public Guid? TypeId { get; set; }
        //public string DocType
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<Tab> Tabs { get; set; }
    }
    public class Tab
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
    public class ItemTagDto
    {
        public long ItemId { get; set; }
        public string Tag { get; set; }
    }
    public class ItemTabDto
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}
