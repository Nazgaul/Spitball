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

        public void UpdateItemsToDirty(long boxId)
        {
            var query1 = UnitOfWork.CurrentSession.GetNamedQuery("UpdateItemToDirty");
            query1.SetInt64("boxId", boxId);
            
            query1.ExecuteUpdate();
            var query2 = UnitOfWork.CurrentSession.GetNamedQuery("UpdateFlashcardToDirty");
            query2.SetInt64("boxId", boxId);
            query2.ExecuteUpdate();
            var query3 = UnitOfWork.CurrentSession.GetNamedQuery("UpdateQuizToDirty");
            query3.SetInt64("boxId", boxId);
            query3.ExecuteUpdate();
        }

        //public int QnACount(long id)
        //{
        //    var questionNumber = UnitOfWork.CurrentSession.Query<Comment>().Count(w => w.Box.Id == id);
        //    return questionNumber;
        //}
      
    }
}
