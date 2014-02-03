using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class BoxTabRepository : NHibernateRepository<BoxTab>, IBoxTabRepository
    {
        public BoxTab GetTabWithTheSameName(string name, long userId)
        {
            var query = UnitOfWork.CurrentSession.QueryOver<BoxTab>();
            query.Where(b => b.User.Id == userId);
            query.Where(b => b.Name == name.Trim().Lower());
            return query.SingleOrDefault<BoxTab>();
        }
    }
}
