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
            return await _session.Query<Question>()
                .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok)
                .OrderByDescending(o => o.Id)

                .Select(s => new QuestionFeedDto
                {
                    Id = s.Id,
                    //User = new UserDto(s.User.Id, s.User.Name, s.User.Score, s.User.Image),
                    Text = s.Text,
                    //Files = s.Attachments,
                    Answers = s.Answers.Where(w => w.Status.State == ItemState.Ok).Count(),
                    DateTime = s.Updated,
                    //HasCorrectAnswer = s.CorrectAnswer.Id != null,
                    CultureInfo = s.Language,
                    //Vote = new VoteDto()
                    //{
                    //    Votes = s.VoteCount
                    //},
                    Course = s.Course.Id
                })
                .Take(50).Skip(query.Page * 50)
                .ToListAsync(token);
        }
    }
}
