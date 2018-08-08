﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    [UsedImplicitly]
    public class QuestionRepository : NHibernateRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(ISession session) : base(session)
        {
        }

        public async Task<IList<Question>> GetAllQuestionsAsync()
        {
            var t = await Session.Query<Question>().ToListAsync();
            return t;
        }

        public async Task<IList<Question>> GetOldQuestionsAsync(CancellationToken token)
        {
            return await Session.Query<Question>().Where(w => w.Updated < DateTime.UtcNow.AddDays(4))
                .Where(w=>w.CorrectAnswer == null)
                .ToListAsync(token).ConfigureAwait(false);
        }

        public Task<Question> GetUserLastQuestionAsync(long userId, CancellationToken token)
        {
            return Session.Query<Question>().Where(w => w.User.Id == userId).OrderByDescending(o => o.Id).Take(1)
                .SingleOrDefaultAsync(cancellationToken: token);
        }
    }
}