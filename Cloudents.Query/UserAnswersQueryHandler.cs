using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public class UserAnswersQueryHandler : IQueryHandler<UserAnswersByIdQuery, IEnumerable<QuestionFeedDto>>
    {
        private readonly IStatelessSession _session;

        public UserAnswersQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(UserAnswersByIdQuery query, CancellationToken token)
        {

            var answerQuery = _session.Query<Answer>()
                .Fetch(f => f.Question)
                .Join(_session.Query<ViewQuestionWithFirstAnswer>(), l => l.Question.Id, r => r.Id, (answer, view) =>
                    new
                    {
                        answer,
                        view
                    });


            return await answerQuery
                .Where(w =>
                    w.answer.User.Id == query.Id
                    && w.answer.Status.State == ItemState.Ok)
                .Select(s => new QuestionFeedDto
                {
                    Id = s.view.Id,
                    Text = s.view.Text,
                    Answers = s.view.Answers,
                    DateTime = s.view.DateTime,
                    Course = s.view.Course,
                    User = new QuestionUserDto()
                    {
                        Id = s.view.User.Id,
                        Name = s.view.User.Name,
                        Image = s.view.User.Image
                    },
                    // UserId = s.view.UserId,
                    FirstAnswer = new AnswerFeedDto()
                    {
                        Text = s.view.Answer.Text,
                        DateTime = s.view.Answer.DateTime,
                        User = new UserDto
                        {
                            Id = s.view.Answer.UserId.GetValueOrDefault(),
                            Image = s.view.Answer.UserImage,
                            Name = s.view.Answer.UserName
                        }
                    }
                }).OrderByDescending(o => o.DateTime).Take(50).Skip(query.Page * 50).ToListAsync(token);
        }
    }
}
