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
    public class UserQuestionsQueryHandler : IQueryHandler<UserDataPagingByIdQuery, IEnumerable<QuestionFeedDto>>
    {
        private readonly IStatelessSession _session;

        public UserQuestionsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(UserDataPagingByIdQuery query, CancellationToken token)
        {
            return await _session.Query<ViewQuestionWithFirstAnswer>()
                .Where(w => w.UserId == query.Id)
                .OrderByDescending(o => o.Id)

                .Select(s => new QuestionFeedDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Answers = s.Answers,
                    DateTime = s.DateTime,
                    CultureInfo = s.CultureInfo,
                    Course = s.Course,
                    UserId = s.UserId,
                    FirstAnswer = s.Answer.UserId == null ? null : new AnswerFeedDto()
                    {
                        Text = s.Answer.Text,
                        DateTime = s.Answer.DateTime,
                        User = new UserDto
                        {
                            Id = s.Answer.UserId.GetValueOrDefault(),
                            Image = s.Answer.UserImage,
                            Name = s.Answer.UserName
                        }
                    }
                })
                .Take(50).Skip(query.Page * 50)
                .ToListAsync(token);
        }
    }
}
