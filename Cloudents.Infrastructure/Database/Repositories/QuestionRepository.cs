﻿using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Repositories
{
    [UsedImplicitly]
    public class QuestionRepository : NHibernateRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(ISession session) : base(session)
        {
        }

        public async Task<IList<Question>> GetAllQuestionsAsync(int page)
        {
            return await Session.Query<Question>().Take(100).Skip(100 * page).OrderBy(o => o.Id).ToListAsync();
        }

        public async Task<IList<Question>> GetOldQuestionsAsync(CancellationToken token)
        {
            //TODO: we can do this better maybe
            return await Session.Query<Question>().Where(w => w.Updated < DateTime.UtcNow.AddDays(4))
                .Where(w => Session.Query<Answer>().FirstOrDefault(x => x.Question.Id == w.Id) == null)
                .ToListAsync(token).ConfigureAwait(false);
        }

        public Task<Question> GetUserLastQuestionAsync(long userId, CancellationToken token)
        {
            return Session.Query<Question>().Where(w => w.User.Id == userId).OrderByDescending(o => o.Id).Take(1)
                .SingleOrDefaultAsync(cancellationToken: token);
        }


        public Task<bool> GetSimilarQuestionAsync(string text, CancellationToken token)
        {
            return Session.Query<Question>().Where(w => w.Text == text.Trim())
                .AnyAsync(token);
        }
    }
}