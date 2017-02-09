using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public abstract class ItemSearchDto
    {
        protected ItemSearchDto()
        {
            Tags = new List<ItemSearchTag>();
        }
        public long Id { get; set; }
        public string Name { get; set; }

        public abstract string Content { get; }
        public string UniversityName { get; set; }
        public string BoxName { get; set; }
        public string BoxCode { get; set; }
        public string BoxProfessor { get; set; }

        public abstract IEnumerable<ItemType> Type { get; }
        public Language? Language { get; set; }

        public List<ItemSearchTag> Tags { get; set; }

        public DateTime Date { get; set; }

        public string BlobName { get; set; }

        public abstract string SearchContentId { get; }// => "item_" + Id;
    }

    public class ItemSearchUsers
    {
        public long Id { get; set; }
    }

    public class ItemSearchTag
    {
        public string Name { get; set; }

        public TagType Type { get; set; }
    }
}