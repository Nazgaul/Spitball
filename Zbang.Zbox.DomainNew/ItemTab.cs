using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain
{
    public class ItemTab
    {
        protected ItemTab()
        {
            Items = new List<Item>();
        }
        public ItemTab(Guid id, string name, Box box)
            : this()
        {
            Id = id;
            Name = name;
            Box = box;
        }
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public virtual Box Box { get; private set; }

        protected virtual ICollection<Item> Items { get; set; }

        public void AddItemToTab(Item item)
        {
            Items.Add(item);
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            Name = name;
        }

        public void DeleteItemFromTab(Item item)
        {
            Items.Remove(item);
        }

        public void DeleteReferenceToItems()
        {
            Items.Clear();
        }

        public override bool Equals(object other)
        {
            //return base.Equals(obj);

            if (this == other) return true;

            var itemTab = other as ItemTab;
            if (itemTab == null) return false;

            if (Name != itemTab.Name) return false;
            if (Box != itemTab.Box) return false;

            return true;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 13 * Name.GetHashCode();
                result += 11 * Box.Id.GetHashCode();
                //result += 19 * UserTime.CreatedUser.GetHashCode();
                //result += 17 * IsDeleted.GetHashCode();
                return result;
            }
        }
    }
}
