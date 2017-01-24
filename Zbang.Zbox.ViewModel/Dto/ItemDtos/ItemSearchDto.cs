using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public abstract class ItemSearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public abstract string Content { get; set; }
        public string UniversityName { get; set; }
        public string BoxName { get; set; }
        public string BoxCode { get; set; }
        public string BoxProfessor { get; set; }

        public abstract ItemType Type { get;  }
        public Language? Language { get; set; }

        public IEnumerable<ItemSearchTag> Tags { get; set; }

        public abstract string SearchContentId { get; }// => "item_" + Id;
    }

    public class DocumentSearchDto: ItemSearchDto
    {
        public string Image { get; set; }

        public override string Content { get; set; }

        public override ItemType Type
        {
            get
            {
                var type = ItemType.Undefined;
                if (TypeDocument.ToLowerInvariant() == "file")
                {
                    type = type | ItemType.Document;
                }
                else
                {
                    type = type | ItemType.Link;
                }
                if (TabName.Equals("Class Notes",StringComparison.InvariantCultureIgnoreCase))
                {
                    return type| ItemType.ClassNote;
                }
                if (TabName.Equals("Homework", StringComparison.InvariantCultureIgnoreCase))
                {
                    return type | ItemType.Homework;
                }
                if (TabName.Equals("Study Guides", StringComparison.InvariantCultureIgnoreCase))
                {
                    return type | ItemType.StudyGuide;
                }
                if (TabName.Equals("Tests & Exams", StringComparison.InvariantCultureIgnoreCase))
                {
                    return type | ItemType.Exam;
                }
                if (TabName.Equals("Lectures", StringComparison.InvariantCultureIgnoreCase))
                {
                    return type | ItemType.Lecture;
                }
                return type;
            }
            
        }

        public override string SearchContentId => "item_" + Id;

        public string Url { get; set; }

        public long? UniversityId { get; set; }

        public string BlobName { get; set; }

        public long BoxId { get; set; }

        public string TabName { get; set; }

        public string TypeDocument { get; set; }

        public IEnumerable<ItemSearchUsers> UserIds { get; set; }

        public override string ToString()
        {
            return $"id : {Id} blobName: {BlobName}";
        }
    }

    public class ItemSearchUsers
    {
        public long Id { get; set; }
    }

    public class ItemSearchTag
    {
        public string Name { get; set; }
    }
}