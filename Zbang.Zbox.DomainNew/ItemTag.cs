using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class ItemTag
    {
        protected ItemTag()
        {

        }

        protected ItemTag(TagType type) : this()
        {
            Type = type;
        }
        public ItemTag(Tag tag, Item item, TagType type) : this(type)
        {

            Tag = tag;
            Item = item;
        }
        public ItemTag(Tag tag, FlashcardMeta item, TagType type) : this(type)
        {

            Tag = tag;
            Flashcard = item;
        }
        public ItemTag(Tag tag, Quiz item, TagType type) : this(type)
        {

            Tag = tag;
            Quiz = item;
        }
        public virtual Guid Id { get; set; }

        public virtual TagType Type { get; set; }

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
            
        }

        private static bool CheckEqualsWithNull(object a, object b)
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


    public class CommentTag
    {
        protected CommentTag()
        {

        }
        public CommentTag(Tag tag, Comment comment, TagType type) : this()
        {

            Tag = tag;
            Comment = comment;
            Type = type;
        }
        
        public virtual Guid Id { get; set; }

        public virtual TagType Type { get; set; }

        public virtual Tag Tag { get; set; }
        public virtual Comment Comment { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as CommentTag;
            if (item == null)
            {
                return false;
            }
            //bool equals = false;
            return CheckEqualsWithNull(Tag, item.Tag)
                   && CheckEqualsWithNull(Comment, item.Comment);

        }

        private static bool CheckEqualsWithNull(object a, object b)
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
                + 13 * Comment?.GetHashCode() ?? 0;
        }
    }
}