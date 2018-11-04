using Cloudents.Core.Entities.Search;
using System;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class DocumentSearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }

        public string Course { get; set; }
        public string Country { get; set; }
        public Guid University { get; set; }
        public DateTime DateTime { get; set; }
        public string Language { get; set; }
        //public string Country { get; set; }

        public Document ToDocument()
        {
            return new Document
            {
                University = University.ToString(),
                DateTime = DateTime,
                Country = Country,
                Course = Course,
                Id = Id.ToString(),
                Name = Name,
                Language = Language,
                Tags = Tags?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}