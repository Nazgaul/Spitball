using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Query.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserUpVotsQueryHandler : IQueryHandler<AdminUserUpVotsQuery, IEnumerable<UserUpVotsDto>>
    {
        private readonly IConfigurationKeys _provider;
        public AdminUserUpVotsQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }
        private const int PageSize = 200;


        public async Task<IEnumerable<UserUpVotsDto>> GetAsync(AdminUserUpVotsQuery query, CancellationToken token)
        {
            const string sql = @"select Id, CreationTime, 
	                                case when V.DocumentId is not null then (select Name from sb.Document where Id = V.DocumentId)
	                                when V.QuestionId is not null then (select Text from sb.Question where Id = V.QuestionId)
	                                when V.AnswerId is not null then (select Text from sb.Answer where Id = V.AnswerId) end as ItemText,
	                                case when V.DocumentId is not null then 'D'
	                                when V.QuestionId is not null then 'Q'
	                                when V.AnswerId is not null then 'A'end as ItemType
                                from sb.Vote V
                                where VoteType = 1 and UserId = @Id 
                                order by 1
                                OFFSET @pageSize * @PageNumber ROWS
                                FETCH NEXT @pageSize ROWS ONLY;";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                return await connection.QueryAsync<UserUpVotsDto>(sql,
                    new
                    {
                        id = query.Id,
                        PageNumber = query.Page,
                        PageSize
                    });
            }
        }
    }
}