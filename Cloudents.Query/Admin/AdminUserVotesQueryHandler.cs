using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserVotesQueryHandler : IQueryHandler<AdminUserVotesQuery, IEnumerable<UserVotesDto>>
    {
        private readonly DapperRepository _dapper;
        public AdminUserVotesQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        private const int PageSize = 200;

        public async Task<IEnumerable<UserVotesDto>> GetAsync(AdminUserVotesQuery query, CancellationToken token)
        {
            const string sql = @"select CreationTime as Created, 
	                                case when V.DocumentId is not null then (select Name from sb.Document where Id = V.DocumentId)
	                                when V.QuestionId is not null then (select Text from sb.Question where Id = V.QuestionId)
	                                when V.AnswerId is not null then (select Text from sb.Answer where Id = V.AnswerId) end as ItemText,
	                                case when V.DocumentId is not null then 'Document'
	                                when V.QuestionId is not null then 'Question'
	                                when V.AnswerId is not null then 'Answer' end as ItemType
                                from sb.Vote V
                                where VoteType = @Type and UserId = @Id 
                                order by 1 desc
                                OFFSET @pageSize * @PageNumber ROWS
                                FETCH NEXT @pageSize ROWS ONLY;";

            using (var connection = _dapper.OpenConnection())
            {
                return await connection.QueryAsync<UserVotesDto>(sql,
                    new
                    {
                        id = query.Id,
                        type = query.Type,
                        PageNumber = query.Page,
                        PageSize
                    });
            }
        }
    }
}