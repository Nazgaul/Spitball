using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    public class UserQuestionsQueryHandler : IQueryHandler<UserQuestionsByIdQuery, IEnumerable<QuestionFeedDto>>
    {
        private readonly ISession _session;

        public UserQuestionsQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }
        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(UserQuestionsByIdQuery query, CancellationToken token)
        {
            var futureQuestions = _session.Query<Question>()
                .Where(w => w.User.Id == query.Id)
                .Where(w => w.State == null || w.State == QuestionState.Ok)
                .OrderByDescending(o => o.Id)
                .Select(s => new QuestionFeedDto(s.Id,
                    s.Subject,
                    s.Price,
                    s.Text,
                    s.Attachments,
                    s.Answers.Count,
                    s.Updated,
                    s.Color, s.CorrectAnswer.Id != null, s.Language)
                )
                .Take(50).Skip(query.Page * 50)
                .ToFuture();
 
            return futureQuestions.GetEnumerable();
        }
    }
}
