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
        public abstract string Name { get; }

        public abstract string Content { get; }
        public ItemUniversitySearchDto University { get; set; }
        public ItemCourseSearchDto Course { get; set; }
       // public string BoxCode { get; set; }
        //public string BoxProfessor { get; set; }
        //public long BoxId { get; set; }

        public abstract IEnumerable<ItemType> Type { get; }
        public Language? Language { get; set; }

        public List<ItemSearchTag> Tags { get; set; }

        public DateTime Date { get; set; }

        public string BlobName { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }

        public abstract string[] MetaContent { get; }

        public abstract int? ContentCount { get; }

        public abstract string SearchContentId { get; }// => "item_" + Id;
    }

    public class ItemCourseSearchDto
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }
        public string Professor { get; set; }

        public override string ToString()
        {
            return $"{Name} {Code} {Professor}";
        }
    }

    public class ItemUniversitySearchDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}