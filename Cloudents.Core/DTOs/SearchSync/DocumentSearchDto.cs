using Cloudents.Core.Entities.Search;
using System;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class DocumentSearchDto
    {
        public long ItemId { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }

        public string Course { get; set; }
        public string Country { get; set; }
        public Guid University { get; set; }
        public DateTime DateTime { get; set; }
        public string Language { get; set; }

        public DocumentType Type { get; set; }
        //public string Country { get; set; }

        public Document ToDocument()
        {
            return new Document
            {
                University = University.ToString(),
                DateTime = DateTime,
                Country = Country,
                Course = Course,
                Id = ItemId.ToString(),
                Name = Name,
                Language = Language,
                Type = Type,
                Tags = Tags?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}