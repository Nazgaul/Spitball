using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;

namespace Cloudents.Infrastructure.Data.Repositories
{
    public class TransactionRepository :NHibernateRepository<Transaction>
    {
        public TransactionRepository(ISession session) : base(session)
        {
        }

        
    }
}