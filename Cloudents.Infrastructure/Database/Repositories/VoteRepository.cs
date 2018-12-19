using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class VoteRepository : NHibernateRepository<Vote>, IVoteRepository
    {
        public VoteRepository(ISession session) : base(session)
        {
        }

        public Task<Vote> GetVoteQuestionAsync(long userId, long questionId, CancellationToken token)
        {
            return Session.Query<Vote>()
                .Where(w => w.User.Id == userId && w.Question.Id == questionId && w.Answer.Id == null)
                .SingleOrDefaultAsync(token);
        }

        public Task<Vote> GetVoteDocumentAsync(long userId, long documentId, CancellationToken token)
        {
            return Session.Query<Vote>().Where(w => w.User.Id == userId && w.Document.Id == documentId)
                .SingleOrDefaultAsync(token);
        }

        public Task<Vote> GetVoteAnswerAsync(long userId, Guid answerId, CancellationToken token)
        {
            return Session.Query<Vote>().Where(w => w.User.Id == userId && w.Answer.Id == answerId)
                .SingleOrDefaultAsync(token);
        }
    }
}