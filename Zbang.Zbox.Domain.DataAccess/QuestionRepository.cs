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
        }


        public float ComputeAverage(long quizId)
        {
            var query = UnitOfWork.CurrentSession.CreateSQLQuery("select avg(score) from zbox.SolvedQuiz where quizid = :quizId ");
            query.SetInt64("quizId", quizId);
           return query.UniqueResult<float>();
        }

    }


}
