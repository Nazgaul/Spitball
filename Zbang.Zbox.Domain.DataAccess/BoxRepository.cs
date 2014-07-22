using Zbang.Zbox.Infrastructure.Data.Repositories;
using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using NHibernate.Linq;
using System.Linq;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class BoxRepository : NHibernateRepository<Box>, IBoxRepository
    {
        public Box GetBoxWithSameName(string name, User user)
        {
            var query = UnitOfWork.CurrentSession.QueryOver<Box>();
            query.Where(b => b.Owner == user);
            query.Where(b => b.IsDeleted == false);
            query.Where(b => b.Name == name.Trim().Lower());
            return query.SingleOrDefault<Box>();
        }

        //public Box Get2(object id)
        //{
        //    var box = Session.Get<Box>(id);
        //    box.Items2 = UnitOfWork.CurrentSession.Query<Item>();
        //    return box;
        //}

        public int QnACount(long id)
        {
            var questionNumber = UnitOfWork.CurrentSession.Query<Comment>().Count(w => w.Box.Id == id);
            var answerNumber = UnitOfWork.CurrentSession.Query<CommentReplies>().Count(w => w.Box.Id == id);
            return questionNumber + answerNumber;
        }
        //public  GetBoxOwner(long boxid)
        //{
        //    UnitOfWork.CurrentSession.QueryOver<
        //}
    }
}
