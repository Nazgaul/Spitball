using System.Linq;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class QuestionRepository : NHibernateRepository<Comment>, IQuestionRepository
    {
        public IQueryable<CommentReplies> GetAnswers(Comment question)
        {

            //this cant be done with query over
            return UnitOfWork.CurrentSession.Query<CommentReplies>().Where(c => c.Question == question);

            //.QueryOver<University>();
            //query.Where(w => w.AliasName.Coalesce(string.Empty) ==  name.Trim().Lower());
            //query.Where()
            //query.OrderBy()
            //      .SqlFunction("coalesce",
            //                            NHibernateUtil.String,
            //                            Projections.Property<University>(x => x.AliasName),
            //                            Projections.Property<University>(x => x.Name)));

            // return query.SingleOrDefault<University>();
        }

        
    }

    
}
