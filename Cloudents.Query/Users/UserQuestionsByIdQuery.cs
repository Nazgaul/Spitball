using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserQuestionsByIdQuery : IQuery<IEnumerable<QuestionFeedDto>>
    {
        public UserQuestionsByIdQuery(long id, int page)
        {
            Id = id;
            Page = page;
        }

        private long Id { get; }
        private int Page { get; }

        internal sealed class UserQuestionsQueryHandler : IQueryHandler<UserQuestionsByIdQuery, IEnumerable<QuestionFeedDto>>
        {
            private readonly IStatelessSession _session;

            public UserQuestionsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<IEnumerable<QuestionFeedDto>> GetAsync(UserQuestionsByIdQuery query, CancellationToken token)
            {
                return await _session.Query<ViewQuestionWithFirstAnswer>()
                    .WithOptions(w => w.SetComment(nameof(UserQuestionsByIdQuery)))
                    .Where(w => w.User.Id == query.Id)
                    .OrderByDescending(o => o.Id)

                    .Select(s => new QuestionFeedDto
                    {
                        Id = s.Id,
                        Text = s.Text,
                        Answers = s.Answers,
                        DateTime = s.DateTime,
                        Course = s.Course,
                        User = new QuestionUserDto()
                        {
                            Id = s.User.Id,
                            Name = s.User.Name,
                            Image = s.User.Image
                        },
                    //UserId = s.UserId,
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
                    }).OrderByDescending(o => o.DateTime)
                    .Take(50).Skip(query.Page * 50)
                    .ToListAsync(token);
            }
        }
    }
}
