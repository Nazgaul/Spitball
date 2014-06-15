using System;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain
{
    public class ItemRate
    {
        protected ItemRate()
        {

        }
        public ItemRate(User user, Item item, Guid id, int rate)
        {
            Throw.OnNull(user, "User");
            Throw.OnNull(item, "Item");
            Id = id;
            User = user;
            Rate = rate;
            Item = item;
        }
        

        public virtual Guid Id { get; private set; }
        public virtual User User { get; private set; }
        public virtual Item Item { get; private set; }
        public virtual int Rate { get; set; }
    }
}
