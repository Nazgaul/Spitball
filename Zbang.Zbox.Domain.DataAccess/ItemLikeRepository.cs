using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ItemLikeRepository : NHibernateRepository<ItemLike>, IItemLikeRepository
    {
        public ItemLike GetItemLike(long userId, long itemId)
        {
            var query = UnitOfWork.CurrentSession.QueryOver<ItemLike>();
            query.Where(b => b.Item.Id == itemId);
            query.Where(b => b.User.Id == userId);
            return query.SingleOrDefault<ItemLike>();
        }
    }
}
