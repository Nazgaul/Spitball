using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    class ItemTabRepository : NHibernateRepository<ItemTab>, IItemTabRepository
    {
        public ItemTab GetTabWithTheSameName(string name, long boxId)
        {
            var query = UnitOfWork.CurrentSession.QueryOver<ItemTab>();
            query.Where(b => b.Box.Id == boxId);
            query.Where(b => b.Name == name.Trim().Lower());
            return query.SingleOrDefault<ItemTab>();
        }
    }
}
