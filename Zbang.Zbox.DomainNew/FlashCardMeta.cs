﻿using System;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public class FlashcardMeta : IDirty
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
        public virtual string Name { get; set; }
        public virtual bool Publish { get; set; }

        public virtual User User { get; set; }
        public virtual int LikeCount { get; set; }
        public virtual int NumberOfViews { get; set; }

        public virtual int CardCount { get; set; }

        public virtual bool IsDeleted { get; set; }
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

        public virtual UserTimeDetails DateTimeUser { get; private set; }

        public virtual Box Box { get; set; }

        public ICollection<FlashcardPin> Pins { get; set; }

    }
}