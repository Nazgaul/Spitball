using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UpdatesRepository : NHibernateRepository<Updates>, IUpdatesRepository
    {
        public IEnumerable<Updates> GetUserBoxUpdates(long userId, long boxId)
        {
            var x = UnitOfWork.CurrentSession.QueryOver<Updates>().
                         Where(w => w.User.Id == userId).Where(w => w.Box.Id == boxId)

                            //.Select(NHibernate.Criterion.Projections.Sum<Item>(s=>s.Size)).SingleOrDefault();
                        .List<Updates>();
            return x;

        }
    }
}
