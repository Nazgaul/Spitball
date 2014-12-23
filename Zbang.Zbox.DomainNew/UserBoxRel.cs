﻿using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class UserBoxRel
    {
        protected UserBoxRel()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            UserTime = new UserTimeDetails("Sys");
            Invites = new List<InviteToBox>();
        }
        public UserBoxRel(User user, Box box, UserRelationshipType userType)
            : this()
        {
            User = user;
            Box = box;
            NotificationSettings = NotificationSettings.OnceADay;
            UserRelationshipType = userType;
            UserId = User.Id;
            BoxId = Box.Id;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            //Star = false;

        }
        public virtual long Id { get; protected set; }
        public virtual UserRelationshipType UserRelationshipType { get; set; }
        public virtual NotificationSettings NotificationSettings { get; set; }
        //public virtual bool Star { get; set; }
        public virtual UserTimeDetails UserTime { get; set; }

        public virtual User User { get; set; }
        public virtual Box Box { get; set; }

        public virtual ICollection<InviteToBox> Invites { get; protected set; }


        //used for hashcode & equals
        protected virtual long UserId { get; set; }
        protected virtual long BoxId { get; set; }

        public override bool Equals(object other)
        {
            if (this == other) return true;

            var userBoxRel = other as UserBoxRel;
            if (userBoxRel == null) return false; // null or not a cat

            if (UserId != userBoxRel.UserId) return false;
            if (BoxId != userBoxRel.BoxId) return false;
            return UserRelationshipType == userBoxRel.UserRelationshipType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = 11 * UserId.GetHashCode();
                result += 13 * BoxId.GetHashCode();
                result += 17 * UserRelationshipType.GetHashCode();
                return result;
            }
        }

    }
}
