using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class AnswerRepository : NHibernateRepository<Answer>, IAnswerRepository
    {
        public AnswerRepository(ISession session) : base(session)
        {
        }

        public Task<int> GetSimilarAnswerInQuestionAsync(string text, long questionId, CancellationToken token)
        {
            return Session.Query<Answer>().Where(w => w.Question.Id == questionId && w.Text == text.Trim())
                .CountAsync(token);
        }

        public Task<int> GetAnswerOfUserAsync(long userId, long questionId, CancellationToken token)
        {
            return Session.Query<Answer>().Where(w => w.Question.Id == questionId && w.User.Id == userId)
                .CountAsync(token);
        }
    }
}