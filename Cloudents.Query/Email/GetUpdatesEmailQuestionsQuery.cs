using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class GetUpdatesEmailQuestionsQuery: IQuery<IEnumerable<QuestionEmailDto>>
    {
        public GetUpdatesEmailQuestionsQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class GetUpdatesEmailQuestionsQueryHandler : IQueryHandler<GetUpdatesEmailQuestionsQuery, IEnumerable<QuestionEmailDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public GetUpdatesEmailQuestionsQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<QuestionEmailDto>> GetAsync(GetUpdatesEmailQuestionsQuery query, CancellationToken token)
            {
                const string sql = @"select u.Id as UserId,
	                                q.Id as QuestionId,
	                                q.Text as QuestionTxt,
	                                u.Name as Asker,
	                                u.Image as UserPicture
                                from sb.Question q
                                join sb.[user] u
	                                on q.UserId = u.Id 
                                where q.Created > getutcdate() - 10 and q.CourseId in (select courseId from sb.UsersCourses where UserId = @UserId)
                                and u.Id != @UserId and q.state = 'Ok'";
                using (var connection = _dapperRepository.OpenConnection())
                {
                    return await connection.QueryAsync<QuestionEmailDto>(sql, new { query.UserId });
                }
            }
        }
    }
}
