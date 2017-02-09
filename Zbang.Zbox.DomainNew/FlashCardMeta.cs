using System;
using System.Collections.Generic;
using System.Linq;
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class FlashcardMeta : IItem, IDirty
    {
        protected FlashcardMeta()
        {
            ShouldMakeDirty = () => true;
        }

        public FlashcardMeta(long id, string name, User user, Box box) : this()
        {
            Id = id;
            Name = name?.Trim();
            User = user;
            Box = box;
            DateTimeUser = new UserTimeDetails(user.Id);
        }
        public virtual long Id { get; set; }
        public virtual Language Language { get; set; }
        public virtual string Name { get; set; }
        public virtual bool Publish { get; set; }

        public virtual User User { get; set; }
        public virtual int LikeCount { get; set; }
        public virtual int NumberOfViews { get; set; }

        public virtual int CardCount { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual Comment Comment { get; set; }
        public void DeleteAssociation()
        {
            Pins.Clear();
            //throw new NotImplementedException();
        }

        public virtual void UpdateNumberOfViews()
        {
            NumberOfViews++;
        }


        public virtual bool IsDirty { get; set; }
        public virtual Func<bool> ShouldMakeDirty { get; set; }
        public virtual ISet<ItemTag> ItemTags { get; set; }

        public virtual void AddTag(Tag tag, TagType type)
        {
            var newExists = ItemTags.FirstOrDefault(w => w.Tag.Id == tag.Id);
            if (newExists != null) return;
            newExists = new ItemTag(tag, this, type);
            ItemTags.Add(newExists);
            tag.ItemTags.Add(newExists);
        }

        public virtual CourseTag CourseTag { get; set; }

        public virtual UserTimeDetails DateTimeUser { get; private set; }

        public virtual Box Box { get; set; }

        public ICollection<FlashcardPin> Pins { get; set; }

    }
}