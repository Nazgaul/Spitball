using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

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
                .Fetch(f => f.Question);

            answerQuery.ThenFetch(f => f.User);

            return await answerQuery
                .Where(w =>
                    w.User.Id == query.Id && w.Status.State == ItemState.Ok && w.Question.Status.State == ItemState.Ok)
                .OrderByDescending(o => o.Question.Id)
                .Select(s=> new QuestionFeedDto()
                    {
                        CultureInfo = s.Question.Language,
                        //User = new UserDto
                        //{
                        //    Id = s.Question.User.Id,
                        //    Name = s.Question.User.Name,
                        //    Score = s.Question.User.Score,
                        //    Image = s.Question.User.Image
                        //},
                        Id = s.Question.Id,
                        Course = s.Question.Course.Id,
                        Text = s.Question.Text,
                        DateTime = s.Question.Updated,
                        //Vote = new VoteDto()
                        //{
                        //    Votes = s.Question.VoteCount
                        //},
                        Answers = s.Question.Answers.Where(w => w.Status.State == ItemState.Ok).Count(),
                        //HasCorrectAnswer = s.Question.CorrectAnswer.Id != null,
                       // Files = s.Question.Attachments
                        
                }).Take(50).Skip(query.Page * 50).ToListAsync(token);
        }
    }
}
