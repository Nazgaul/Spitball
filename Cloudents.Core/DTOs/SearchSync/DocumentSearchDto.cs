using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs.SearchSync
{
    public class DocumentSearchDto
    {
        [DtoToEntityConnection(nameof(Document.Id))]
        public long ItemId { get; set; }
        [DtoToEntityConnection(nameof(Document.Id))]
        public string Name { get; set; }

        public string Tags
        {
           // get => _tags;
            set => TagsArray = value?.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

        [DtoToEntityConnection(nameof(Document.Id))]
        public string[] TagsArray { get; set; }

        [DtoToEntityConnection(nameof(Document.Course.Name))]
        public string Course { get; set; }
        [DtoToEntityConnection(nameof(Document.User.Country))]
        public string Country { get; set; }
        [DtoToEntityConnection(nameof(Document.University.Id))]
        public Guid? UniversityId { get; set; }
        [DtoToEntityConnection(nameof(Document.University.Name))]
        public string UniversityName { get; set; }
        [DtoToEntityConnection(nameof(Document.TimeStamp.UpdateTime))]
        public DateTime? DateTime { get; set; }
        [DtoToEntityConnection(nameof(Document.Status.State))]
        public ItemState State { get; set; }
        [DtoToEntityConnection(nameof(Document.Type))]
        public DocumentType Type { get; set; }

       
    }
}