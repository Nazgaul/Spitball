//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using Cloudents.Query.Query;
//using NHibernate;
//using NHibernate.Linq;
//using System.Diagnostics.CodeAnalysis;
//using System.Globalization;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query
//{
//    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Injected")]
//    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
//    public class QuestionDetailQueryHandler : IQueryHandler<QuestionDataByIdQuery, QuestionDetailDto>
//    {
//        private readonly IStatelessSession _session;

//        public QuestionDetailQueryHandler(QuerySession session)
//        {
//            _session = session.StatelessSession;
//        }

//        private async Task<QuestionDetailDto> GetFromDbAsync(long id, CancellationToken token)
//        {

//            var questionFuture = _session.Query<Question>()
//                .Where(w => w.Id == id && w.Status.State == ItemState.Ok)
//                //.Fetch(f => f.User)
//                .Select(s => new QuestionDetailDto
//                {
//                    //User = new UserDto
//                    //{
//                    //    Id = s.User.Id,
//                    //    Name = s.User.Name,
//                    //    Image = s.User.Image,
//                    //    Score = s.User.Score
//                    //},
//                    Id = s.Id,
//                    Course = s.Course.Id,

//                    Text = s.Text,
//                    //CorrectAnswerId = s.CorrectAnswer.Id,
//                    Create = s.Updated,
//                    User = new QuestionUserDto()
//                    {
//                        Id = s.User.Id,
//                        Name = s.User.Name,
//                        Image = s.User.Image
//                    }
//                }

//                ).ToFutureValue();
//            var answersFuture = _session.Query<Answer>()
//                .Where(w => w.Question.Id == id && w.Status.State == ItemState.Ok)
//                .Fetch(f => f.User)
//                .Select(s => new QuestionDetailAnswerDto
//                (
//                    s.Id,
//                    s.Text,
//                    new UserDto
//                    {
//                        Id = s.User.Id,
//                        Name = s.User.Name,
//                        Image = s.User.Image,
//                        Score = s.User.Score
//                    },
//                    s.Created

//                    //new VoteDto
//                    //{
//                    //   // Votes = s.VoteCount
//                    //},

                    


//        )).ToFuture();

//            var dto = await questionFuture.GetValueAsync(token);
//            if (dto == null)
//            {
//                return null;
//            }
//            var answerResult = await answersFuture.GetEnumerableAsync(token);


//            dto.Answers = answerResult.OrderByDescending(x => x.Create);


//            return dto;


//        }

//        private static bool SetIsRtl(CultureInfo info)
//        {
//            return info?.TextInfo.IsRightToLeft ?? false;
//        }

//        public async Task<QuestionDetailDto> GetAsync(QuestionDataByIdQuery query, CancellationToken token)
//        {
//            var dto = await GetFromDbAsync(query.Id, token);

//            if (dto == null)
//            {
//                return null;
//            }
//            dto.Answers = dto.Answers;

//            return dto;
//        }


//    }




//}