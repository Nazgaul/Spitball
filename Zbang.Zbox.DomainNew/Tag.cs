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
            ItemTags = new HashSet<ItemTag>();
        }
        public Tag(string name) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name cant be null");
            }
            Name = name;
        }

        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ISet<ItemTag> ItemTags { get; set; }
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
        public ItemTag(Tag tag, FlashcardMeta item) : this()
        {

            Tag = tag;
            Flashcard = item;
        }
        public ItemTag(Tag tag, Quiz item) : this()
        {

            Tag = tag;
            Quiz = item;
        }
        public virtual Guid Id { get; set; }

        public virtual Tag Tag { get; set; }
        public virtual Item Item { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual FlashcardMeta Flashcard { get; set; }


        public override bool Equals(object obj)
        {
            var item = obj as ItemTag;
            if (item == null)
            {
                return false;
            }
            //bool equals = false;
            return CheckEqualsWithNull(Tag, item.Tag)
                   && CheckEqualsWithNull(Item, item.Item)
                   && CheckEqualsWithNull(Item, item.Item)
                   && CheckEqualsWithNull(Flashcard, item.Flashcard)
                   && CheckEqualsWithNull(Quiz, item.Quiz);
            //return  Tag.Equals(item.Tag) && Item.Equals(item.Item) 
            // && Flashcard.Equals(item.Flashcard) && Quiz.Equals(item.Quiz);
        }

        private bool CheckEqualsWithNull(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a != null && b == null)
            {
                return false;
            }
            if (a == null)
            {
                return false;
            }
            return a.Equals(b);

        }

        public override int GetHashCode()
        {
            return
                11 * Tag.GetHashCode()
                + 13 * Item?.GetHashCode() ?? 0
                + 17 * Flashcard?.GetHashCode() ?? 0
            + 19 * Quiz?.GetHashCode() ?? 0;
        }
    }
}
