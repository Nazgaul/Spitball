using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ItemRateRepository : NHibernateRepository<ItemRate>, IItemRateRepository
    {
        public int GetRateCount(long itemId)
        {
            return UnitOfWork.CurrentSession.Query<ItemRate>().Where(w => w.Item.Id == itemId).Count();
        }

        public ItemRate GetRateOfUser(long userId, long itemId)
        {
            return UnitOfWork.CurrentSession.Query<ItemRate>().Where(w => w.Item.Id == itemId && w.User.Id == userId).FirstOrDefault();
        }
    }
}
