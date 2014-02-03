using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class QuestionRepository : NHibernateRepository<Question>, IQuestionRepository
    {
        public IQueryable<Answer> GetAnswers(Question question)
        {

            //this cant be done with query over
            return UnitOfWork.CurrentSession.Query<Answer>().Where(c => c.Question == question);

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
