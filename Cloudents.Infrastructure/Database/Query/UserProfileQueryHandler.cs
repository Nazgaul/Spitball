using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserProfileQueryHandler : IQueryHandler<UserDataByIdQuery, ProfileDto>
    {
        private readonly ISession _session;

        public UserProfileQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }

        public async Task<ProfileDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            var futureDto = _session.Query<User>()
                 .Fetch(u => u.University)
                 .Where(w => w.Id == query.Id)
                 .Select(s => new ProfileDto
                 {
                     User = new UserProfileDto
                     {
                         Id = s.Id,
                         Image = s.Image,
                         Name = s.Name,
                         UniversityName = s.University.Name
                     }
                 })
                 .ToFutureValue();
            var futureQuestions = _session.Query<Question>()
                .Where(w => w.User.Id == query.Id)
                .Where(w => w.State == null || w.State == ItemState.Ok)
                .OrderByDescending(o => o.Id)
                .Select(s => new QuestionFeedDto(s.Id,
                    s.Subject,
                    s.Price,
                    s.Text,
                    s.Attachments,
                    s.Answers.Count,
                    s.Updated,
                    s.Color, s.CorrectAnswer.Id != null, s.Language)
                ).ToFuture();

            var answerQuery = _session.Query<Answer>()
                .Fetch(f => f.Question);

            answerQuery.ThenFetch(f => f.User);

            var futureAnswers = answerQuery.Where(w => w.User.Id == query.Id)
                .OrderByDescending(o => o.Question.Id)
                .Select(s=> new QuestionFeedDto(s.Question.Id,
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

                .ToFuture();



            var dto = await futureDto.GetValueAsync(token).ConfigureAwait(false);
            if (dto == null)
            {
                return null;
            }

            dto.Questions = futureQuestions.GetEnumerable();
            dto.Answers = futureAnswers.GetEnumerable();

            return dto;
        }
    }
}