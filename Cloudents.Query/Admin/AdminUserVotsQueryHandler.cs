using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserVotsQueryHandler : IQueryHandler<AdminUserVotsQuery, IEnumerable<UserVotsDto>>
    {
        private readonly IConfigurationKeys _provider;
        public AdminUserVotsQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }
        private const int PageSize = 200;

        public async Task<IEnumerable<UserVotsDto>> GetAsync(AdminUserVotsQuery query, CancellationToken token)
        {
            const string sql = @"select CreationTime as Created, 
	                                case when V.DocumentId is not null then (select Name from sb.Document where Id = V.DocumentId)
	                                when V.QuestionId is not null then (select Text from sb.Question where Id = V.QuestionId)
	                                when V.AnswerId is not null then (select Text from sb.Answer where Id = V.AnswerId) end as ItemText,
	                                case when V.DocumentId is not null then 'D'
	                                when V.QuestionId is not null then 'Q'
	                                when V.AnswerId is not null then 'A'end as ItemType
                                from sb.Vote V
                                where VoteType = @Type and UserId = @Id 
                                order by 1 desc
                                OFFSET @pageSize * @PageNumber ROWS
                                FETCH NEXT @pageSize ROWS ONLY;";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                return await connection.QueryAsync<UserVotsDto>(sql,
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