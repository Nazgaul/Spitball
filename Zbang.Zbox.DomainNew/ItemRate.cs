using System;

namespace Zbang.Zbox.Domain
{
    public class ItemRate
    {
        protected ItemRate()
        {

        }

        public ItemRate(User user, Item item, Guid id)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            Id = id;
            User = user;
            Rate = 1;
            Item = item;
            CreationTime = DateTime.UtcNow;
        }
        

        public virtual Guid Id { get; private set; }
        public virtual User User { get; private set; }
        public virtual Item Item { get; private set; }
        public virtual int Rate { get; private set; }

        public virtual DateTime CreationTime { get; private set; }
    }
}
