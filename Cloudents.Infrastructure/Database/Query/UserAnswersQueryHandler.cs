using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
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
    public class UserAnswersQueryHandler : IQueryHandler<UserAnswersByIdQuery, IEnumerable<QuestionFeedDto>>
    {
        private readonly ISession _session;

        public UserAnswersQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }
        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(UserAnswersByIdQuery query, CancellationToken token)
        {

            var answerQuery = _session.Query<Answer>()
                .Fetch(f => f.Question);

            answerQuery.ThenFetch(f => f.User);

            var futureAnswers = answerQuery.Where(w => w.User.Id == query.Id)
                .OrderByDescending(o => o.Question.Id)
                .Select(s => new QuestionFeedDto(s.Question.Id,
                    s.Question.Subject,
                    s.Question.Price,
                    s.Question.Text,
                    s.Question.Attachments,
                    s.Question.Answers.Count,
                    new UserDto()
                    {
                        Id = s.Question.User.Id,
                        Name = s.Question.User.Name,
                        Image = s.Question.User.Image
                    }, s.Question.Updated,
                    s.Question.Color, s.Question.CorrectAnswer.Id != null, s.Question.Language))
                    .Take(50).Skip(query.Page*50)
                    .ToFuture();

            return futureAnswers.GetEnumerable();
        }
    }
}
