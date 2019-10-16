using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Query
{
    [UsedImplicitly]
    public class QuestionsQueryHandler : IQueryHandler<IdsQuestionsQuery<long>, IEnumerable<QuestionFeedDto>>
    {
        private readonly IDapperRepository _dapperRepository;

        public QuestionsQueryHandler(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(IdsQuestionsQuery<long> query, CancellationToken token)
        {

            const string sql = @"SELECT  q.Id as Id,
  q.Subject_id as Subject,
  q.Price as Price,
   q.Text as Text,
    q.Attachments as Files,
	 q.CourseId as Course,
	  (SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE (this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
	   q.Updated as DateTime,
		    (case when not (q.CorrectAnswer_id is null) then 1 else 0 end) as HasCorrectAnswer,
			 q.Language as CultureInfo,
		u.Id as Id,
	   u.Name as Name,
		 u.Score as Score,
		  u.Image as Image,
 q.VoteCount as Votes
			  FROM sb.[Question] q 
			  inner join sb.[User] u on q.UserId=u.Id
			   WHERE q.Id in @ids
			   and q.State = 'ok';";

            using (var conn = _dapperRepository.OpenConnection())
            {
                var retVal = await conn.QueryAsync<QuestionFeedDto, UserDto, VoteDto, QuestionFeedDto>(sql, (feedDto, userDto, voteDto) =>
             {
                 //feedDto.User = userDto;
                 //feedDto.Vote = voteDto;
                 return feedDto;
             }, new { ids = query.QuestionIds }, splitOn: "Id,Votes");

                return retVal;
            }
        }
    }
}