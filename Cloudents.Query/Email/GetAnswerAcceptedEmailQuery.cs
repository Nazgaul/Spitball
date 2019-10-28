//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.DTOs;
//using Dapper;

//namespace Cloudents.Query.Email
//{
//    public class GetAnswerAcceptedEmailQuery : IQuery<AnswerAcceptedEmailDto>
//    {
//        public GetAnswerAcceptedEmailQuery(long questionId)
//        {
//            QuestionId = questionId;
//        }

//        private long QuestionId { get; }


//        internal sealed class GetAnswerAcceptedEmailQueryQueryHandler : IQueryHandler<GetAnswerAcceptedEmailQuery, AnswerAcceptedEmailDto>
//        {
//            private readonly IDapperRepository _dapper;

//            public GetAnswerAcceptedEmailQueryQueryHandler(IDapperRepository dapper)
//            {
//                _dapper = dapper;
//            }

//            public async Task<AnswerAcceptedEmailDto> GetAsync(GetAnswerAcceptedEmailQuery query, CancellationToken token)
//            {
//                const string sql = @"Select 
//u.Email as ToEmailAddress,
//u.Language,
// u.id as userId,
// q.Text as questionText,
//q.Id as questionId,
// a.Text as answerText
//  from  sb.Question q 

//join sb.Answer a on a.Id = q.CorrectAnswer_id
//join sb.[User] u on a.UserId = u.Id
//where q.id = @id";
//                using (var connection = _dapper.OpenConnection())
//                {
//                    return await connection.QuerySingleAsync<AnswerAcceptedEmailDto>(sql,
//                        new
//                        {
//                            id = query.QuestionId,
//                        });
//                }
//            }
//        }
//    }
//}