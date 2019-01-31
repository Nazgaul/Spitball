using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Data;
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
    public class AdminUserQuestionsQueryHandler : IQueryHandler<AdminUserQuestionsQuery, IEnumerable<UserQuestionsDto>>
    {
        private readonly IConfigurationKeys _provider;


        public AdminUserQuestionsQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }
        private const int pageSize = 200;

        public async Task<IEnumerable<UserQuestionsDto>> GetAsync(AdminUserQuestionsQuery query, CancellationToken token)
        {
            var sql = @"select Id, Text, Created, [State]
                from sb.Question 
                where UserId = @Id
				order by 1
                 OFFSET @PageSize * (@PageNumber - 1) ROWS
                 FETCH NEXT @PageSize ROWS ONLY;";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                return await connection.QueryAsync<UserQuestionsDto>(sql,
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
