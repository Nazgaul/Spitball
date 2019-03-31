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
    public class AdminUserQuestionsQueryHandler : IQueryHandler<AdminUserQuestionsQuery, IEnumerable<UserQuestionsDto>>
    {
        private readonly DapperRepository _dapper;


        public AdminUserQuestionsQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        private const int PageSize = 200;

        public async Task<IEnumerable<UserQuestionsDto>> GetAsync(AdminUserQuestionsQuery query, CancellationToken token)
        {
            const string sql = @"select Id, Text, Created, [State]
                from sb.Question 
                where UserId = @Id
				order by 1
                 OFFSET @PageSize * @PageNumber ROWS
                 FETCH NEXT @PageSize ROWS ONLY;";
            return await _dapper.WithConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<UserQuestionsDto>(sql,
                    new
                    {
                        id = query.UserId,
                        PageNumber = query.Page,
                        PageSize
                    });
            }, token);
        }
    }
}
