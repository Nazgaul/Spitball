﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain
{
    public class FlashcardMeta : IDirty, ITag, ILanguage
    {
        protected FlashcardMeta()
        {
            ShouldMakeDirty = () => false;
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
            ShouldMakeDirty = () => true;
        }

        public virtual bool IsDirty { get; set; }
        public virtual Func<bool> ShouldMakeDirty { get; set; }
        public virtual ISet<ItemTag> ItemTags { get; set; }

        public virtual Task AddTagAsync(Tag tag, TagType type, IJaredPushNotification jaredPush)
        {
            var newExists = ItemTags.FirstOrDefault(w => w.Tag.Id == tag.Id);
            if (newExists != null) return Task.CompletedTask;
            newExists = new ItemTag(tag, this, type);
            ItemTags.Add(newExists);
            tag.ItemTags.Add(newExists);
            ShouldMakeDirty = () => true;
            if (type != TagType.Watson)
            {
                ShouldMakeDirty = () => true;
            }
            if (DateTimeUser.CreationTime.AddDays(1) > DateTime.UtcNow)
            {
                return jaredPush.SendItemPushAsync(User.Name, Box.Id, Id, tag.Name, ItemType.Flashcard);
            }
            return Task.CompletedTask;
        }

        public virtual void RemoveTag(string tag)
        {
            var tagToRemove = ItemTags.FirstOrDefault(w => w.Tag.Name == tag);
            if (tagToRemove == null) return;
            ItemTags.Remove(tagToRemove);
            ShouldMakeDirty = () => true;
        }

        // public virtual CourseTag CourseTag { get; set; }

        public virtual UserTimeDetails DateTimeUser { get; private set; }

        public virtual Box Box { get; set; }

        public ICollection<FlashcardPin> Pins { get; set; }
    }
}