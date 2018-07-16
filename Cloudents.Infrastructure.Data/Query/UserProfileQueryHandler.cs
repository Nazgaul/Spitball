using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserProfileQueryHandler : IQueryHandler<UserDataByIdQuery, ProfileDto>
    {
        private readonly ISession _session;

        public UserProfileQueryHandler(ReadonlySession session)
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
                .Fetch(f => f.Subject)
                .Where(w => w.User.Id == query.Id)
                .OrderByDescending(o => o.Id)
                .Select(s => new QuestionDto
                {
                    Text = s.Text,
                    Answers = s.Answers.Count,
                    DateTime = s.Created,
                    Files = s.Attachments,
                    Price = s.Price,
                    Id = s.Id,
                    Subject = s.Subject.Text
                }).ToFuture();

            var answerQuery = _session.Query<Answer>()
                .Fetch(f => f.Question);

            answerQuery.ThenFetch(f => f.Subject);
            answerQuery.ThenFetch(f => f.User);


            //var futureAnswers = _session.Query<Answer>()
            //    .Fetch(f => f.Question)

            //    .ThenFetch(f => f.Subject)

            var futureAnswers = answerQuery.Where(w => w.User.Id == query.Id)
                .OrderByDescending(o => o.Question.Id)
                .Select(s => new QuestionDto
                {
                    Text = s.Question.Text,
                    Answers = s.Question.Answers.Count,
                    DateTime = s.Question.Created,
                    Files = s.Question.Attachments,
                    Price = s.Question.Price,
                    Id = s.Question.Id,
                    Subject = s.Question.Subject.Text,
                    User = new UserDto
                    {
                        Id = s.Question.User.Id,
                        Name = s.Question.User.Name,
                        Image = s.Question.User.Image
                    }
                }).ToFuture();

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