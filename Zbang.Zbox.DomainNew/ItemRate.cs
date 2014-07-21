using System;

namespace Zbang.Zbox.Domain
{
    public class ItemRate
    {
        protected ItemRate()
        {

        }
        public ItemRate(User user, Item item, Guid id, int rate)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
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
