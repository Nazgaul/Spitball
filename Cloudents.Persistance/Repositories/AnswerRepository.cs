﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Persistance.Repositories
{
    public class AnswerRepository : NHibernateRepository<Answer>, IAnswerRepository
    {
        public AnswerRepository(ISession session) : base(session)
        {
        }

       
        public Task<Answer> GetUserAnswerInQuestion(long questionId, long userId, CancellationToken token)
        {
            return Session.Query<Answer>()
                .Where(w => w.Question.Id == questionId && w.User.Id == userId && w.Item.State == ItemState.Ok)
                .SingleOrDefaultAsync(token);

        }
    }
}