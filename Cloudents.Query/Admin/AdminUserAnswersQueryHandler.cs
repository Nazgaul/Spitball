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
    public class AdminUserAnswersQueryHandler : IQueryHandler<AdminUserAnswersQuery, IEnumerable<UserAnswersDto>>
    {
        private readonly IConfigurationKeys _provider;


        public AdminUserAnswersQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }
        private const int pageSize = 200;

        public async Task<IEnumerable<UserAnswersDto>> GetAsync(AdminUserAnswersQuery query, CancellationToken token)
        {
            var sql = @"select A.Id, A.Text, A.Created, A.QuestionId, Q.Text as QuestionText, A.[State]
                from sb.Answer A
                join sb.Question Q
	                on A.QuestionId = Q.Id
                where A.UserId = @Id
                order by 1
                OFFSET @pageSize * (@PageNumber - 1) ROWS
                FETCH NEXT @pageSize ROWS ONLY;";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                return await connection.QueryAsync<UserAnswersDto>(sql,
                    new
                    {
                        id = query.UserId,
                        PageNumber = query.Page,
                        PageSize = pageSize
                    });
            }

        }
    }
}
