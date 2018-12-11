using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class AnswerRepository : NHibernateRepository<Answer>, IAnswerRepository
    {
        public AnswerRepository(ISession session) : base(session)
        {
        }

        //public Task<int> GetNumberOfPendingAnswer(long userId, CancellationToken token)
        //{
        //    return Session.Query<Answer>()
        //         .Fetch(f => f.Question)
        //         .Where(w => w.User.Id == userId)
        //         .Where(w => w.Question.CorrectAnswer.Id == null && w.Question.Created > DateTime.UtcNow.AddDays(-7))
        //         .CountAsync(token);

        //}
    }
}