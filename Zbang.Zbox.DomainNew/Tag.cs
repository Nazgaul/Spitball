using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Domain
{
    public class Tag
    {
        protected Tag()
        {

        }
        public Tag(string name) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name cant be null");
            }
            Name = name;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public ISet<ItemTag> ItemTags { get; set; }
    }

    public class ItemTag
    {
        protected ItemTag()
        {

        }
        public ItemTag(Tag tag, Item item) : this()
        {

            Tag = tag;
            Item = item;


        }
        public Guid Id { get; set; }

        public Tag Tag { get; set; }
        public Item Item { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as ItemTag;
            if (item == null)
            {
                return false;
            }
            return Tag.Equals(item.Tag) && Item.Equals(item.Item);
        }

        public override int GetHashCode()
        {
            return 11 * Tag.GetHashCode() + 13 * Item.GetHashCode();
        }
    }
}
