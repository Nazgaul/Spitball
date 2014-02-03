using System;
using System.Collections.Generic;

namespace Zbang.Zbox.Domain
{
    public class BoxTab
    {
        protected BoxTab()
        {
            UserBoxRel = new Iesi.Collections.Generic.HashedSet<UserBoxRel>();
        }
        public BoxTab(Guid id, string name, User user)
            : this()
        {
            Id = id;
            Name = name;
            User = user;
        }
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public virtual User User { get; private set; }

        protected virtual ICollection<UserBoxRel> UserBoxRel { get; set; }

        public void AddBoxToTag(UserBoxRel userBoxRel)
        {
            UserBoxRel.Add(userBoxRel);
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            Name = name;
        }

        public void DeleteReferenceToBoxes()
        {
            UserBoxRel.Clear();
        }

        public override bool Equals(object other)
        {
            //return base.Equals(obj);

            if (this == other) return true;

            var boxTab = other as BoxTab;
            if (boxTab == null) return false;

            if (Name != boxTab.Name) return false;
            if (User != boxTab.User) return false;

            return true;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 13 * Name.GetHashCode();
                result += 11 * User.Id.GetHashCode();
                //result += 19 * UserTime.CreatedUser.GetHashCode();
                //result += 17 * IsDeleted.GetHashCode();
                return result;
            }
        }
    }
}
