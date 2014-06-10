using System.Linq;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ItemRateRepository : NHibernateRepository<ItemRate>, IItemRateRepository
    {
        public int GetRateCount(long itemId)
        {
            return UnitOfWork.CurrentSession.Query<ItemRate>().Count(w => w.Item.Id == itemId);
        }

        public ItemRate GetRateOfUser(long userId, long itemId)
        {
            return UnitOfWork.CurrentSession.Query<ItemRate>().FirstOrDefault(w => w.Item.Id == itemId && w.User.Id == userId);
        }
    }
}
