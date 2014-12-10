using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Linq;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class BoxRepository : NHibernateRepository<Box>, IBoxRepository
    {
        public Box GetBoxWithSameName(string name, User user)
        {
            var query = UnitOfWork.CurrentSession.QueryOver<Box>();
            query.Where(b => b.Owner.Id == user.Id);
            query.Where(b => b.IsDeleted == false);
            query.Where(b => b.Name == name.Trim().Lower());
            return query.SingleOrDefault<Box>();
        }

        public int QnACount(long id)
        {
            var questionNumber = UnitOfWork.CurrentSession.Query<Comment>().Count(w => w.Box.Id == id);
            return questionNumber;
        }
      
    }
}
