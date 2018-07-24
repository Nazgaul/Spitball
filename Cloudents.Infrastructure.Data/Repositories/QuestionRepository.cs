using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;
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
    }
}