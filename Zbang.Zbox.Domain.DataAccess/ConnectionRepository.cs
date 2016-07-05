using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class ConnectionRepository : NHibernateRepository<Connection>, IConnectionRepository
    {
        public IList<Connection> GetZombies()
        {
            DateTimeOffset someTime = DateTimeOffset.UtcNow;
            var sqlString = " DATEDIFF( ss, [LastActivity], ? ) >= 30 ";
            var sql = NHibernate.Criterion.Expression.Sql(sqlString
                          , someTime
                          , NHibernate.NHibernateUtil.DateTimeOffset);

            return UnitOfWork.CurrentSession.QueryOver<Connection>()
                  .Where(sql)
                  .List();
        }
    }

    public interface IConnectionRepository : IRepository<Connection>
    {
        IList<Connection> GetZombies();
    }
}
