using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class ItemLike
    {
        protected ItemLike()
        {

        }
        public ItemLike(User user, Item item)
        {
            Throw.OnNull(user, "User");
            Throw.OnNull(item, "Item");
            var idGenerator = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<IIdGenerator>();
            Id = idGenerator.GetId(IdGenerator.ItemLikeScope);


            User = user;
            Item = item;
            Like = true;
            IncrementCount();
        }

        public void ChangeUserLike()
        {
            Like = !Like;
            if (Like)
            {
                IncrementCount();
            }
            else
            {
                Item.LikeCount--;
            }
        }

        public  void IncrementCount()
        {
            Item.LikeCount++;
        }


        public virtual long Id { get; private set; }
        public virtual User User { get; private set; }
        public virtual Item Item { get; private set; }

        public virtual bool Like { get; set; }

    }
}
