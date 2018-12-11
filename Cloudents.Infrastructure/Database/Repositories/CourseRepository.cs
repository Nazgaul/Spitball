using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class CourseRepository : NHibernateRepository<Course>, ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {
        }

        public async Task<Course> GetOrAddAsync(string name, CancellationToken token)
        {
            if (name == null)
            {
                throw new System.ArgumentNullException(nameof(name));
            }
            name = name.Trim();
            var course = await GetAsync(name, token);

            if (course == null)
            {

                course = new Course(name);
                await AddAsync(course, token).ConfigureAwait(true);
            }

            return course;
        }
    }

    public class VoteRepository : NHibernateRepository<Vote>, IVoteRepository
    {
        public VoteRepository(ISession session) : base(session)
        {
        }

        public Task<Vote> GetVoteQuestionAsync(long userId, long questionId, CancellationToken token)
        {
            return Session.Query<Vote>().Where(w => w.User.Id == userId && w.Question.Id == questionId)
                .SingleOrDefaultAsync(cancellationToken: token);
        }

        public Task<Vote> GetVoteDocumentAsync(long userId, long documentId, CancellationToken token)
        {
            return Session.Query<Vote>().Where(w => w.User.Id == userId && w.Document.Id == documentId)
                .SingleOrDefaultAsync(cancellationToken: token);
        }

        public Task<Vote> GetVoteAnswerAsync(long userId, Guid answerId, CancellationToken token)
        {
            return Session.Query<Vote>().Where(w => w.User.Id == userId && w.Answer.Id == answerId)
                .SingleOrDefaultAsync(cancellationToken: token);
        }
    }
}