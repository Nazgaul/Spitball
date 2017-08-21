using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class Tag
    {
        protected Tag()
        {
            ItemTags = new HashSet<ItemTag>();
            CommentTags = new HashSet<CommentTag>();
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
        public virtual ISet<CommentTag> CommentTags { get; set; }
    }
}
