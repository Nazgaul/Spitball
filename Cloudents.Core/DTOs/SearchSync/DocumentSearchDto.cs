using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class DocumentSearchDto
    {
        public long ItemId { get; set; }
        public string Name { get; set; }

        public string Tags
        {
           // get => _tags;
            set => TagsArray = value?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] TagsArray { get; set; }

        public string Course { get; set; }
        public string Country { get; set; }
        public Guid? UniversityId { get; set; }
        public string UniversityName { get; set; }
        public DateTime? DateTime { get; set; }
        public ItemState State { get; set; }
        public string Type { get; set; }

       
    }
}