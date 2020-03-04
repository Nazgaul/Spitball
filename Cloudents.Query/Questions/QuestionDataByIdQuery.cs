using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.DTOs.Questions;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Questions
{
    public class QuestionDataByIdQuery : IQuery<QuestionDetailDto>

    {
        public QuestionDataByIdQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }


        internal sealed class QuestionDetailQueryHandler : IQueryHandler<QuestionDataByIdQuery, QuestionDetailDto>
        {
            private readonly IStatelessSession _session;

            public QuestionDetailQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }


            //TODO We need to delete the cache upon adding deleting answer
            //[Cache(TimeConst.Minute*10,"question-detail",false)]
            public async Task<QuestionDetailDto> GetAsync(QuestionDataByIdQuery query, CancellationToken token)
            {
                var questionFuture = _session.Query<Question>()
                    .Where(w => w.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Fetch(f => f.User)
                    .Select(s => new QuestionDetailDto
                        {
                            Id = s.Id,
                            Course = s.Course.Id,
                            Text = s.Text,
                            Create = s.Updated,
                            User = new QuestionUserDto()
                            {
                                Id = s.User.Id,
                                Name = s.User.Name,
                                Image = s.User.ImageName
                            }
                        }

                    ).ToFutureValue();
                var answersFuture = _session.Query<Answer>()
                    .Where(w => w.Question.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Fetch(f => f.User)
                    .OrderByDescending(x => x.Created)
                    .Select(s => new QuestionDetailAnswerDto
                    (
                        s.Id,
                        s.Text,
                        new UserDto
                        {
                            Id = s.User.Id,
                            Name = s.User.Name,
                            Image = s.User.ImageName,
                        },
                        s.Created
                    )).ToFuture();

                var dto = await questionFuture.GetValueAsync(token);
                if (dto == null)
                {
                    return null;
                }

                dto.Answers = answersFuture.GetEnumerable().ToList();
                return dto;
            }


        }
    }
}