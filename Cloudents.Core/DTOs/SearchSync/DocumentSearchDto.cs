using Cloudents.Core.Entities.Search;
using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class DocumentSearchDto
    {
        private string[] _tags;
        public long ItemId { get; set; }
        public string Name { get; set; }

        public string Tags
        {
           // get => _tags;
            set
            {
                _tags = value?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                
            }
        }

        public string[] TagsArray
        {
            get => _tags;
            set => _tags = value;
        }

        public string Course { get; set; }
        public string Country { get; set; }
        public Guid University { get; set; }
        public DateTime DateTime { get; set; }
        public string Language { get; set; }
        public ItemState State { get; set; }
        public DocumentType Type { get; set; }

       
    }
}