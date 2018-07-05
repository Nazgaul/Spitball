using Cloudents.Core.Entities.Db;
using NHibernate;

namespace Cloudents.Infrastructure.Data.Repositories
{
    public class QuestionSubjectRepository 
    {
        internal IQueryOver<QuestionSubject, QuestionSubject> GetSubjects(IQueryOver<QuestionSubject, QuestionSubject> query)
        {
            return query.OrderBy(o => o.Text).Asc;
        }


    }
}