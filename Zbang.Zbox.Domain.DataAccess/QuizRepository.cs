using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class QuizRepository : NHibernateRepository<Quiz>, IQuizRepository
    {
        public int ComputeAverage(long quizId)
        {
            var query = UnitOfWork.CurrentSession.CreateSQLQuery("select avg(score) from zbox.SolvedQuiz where quizid = :quizId ");
            query.SetInt64("quizId", quizId);
            return query.UniqueResult<int>();
        }

        public double ComputeStdevp(long quizId)
        {
            var query = UnitOfWork.CurrentSession.CreateSQLQuery("select stdevp(score) from zbox.SolvedQuiz where quizid = :quizId ");
            query.SetInt64("quizId", quizId);
            return query.UniqueResult<double>();
        }
    }
}
