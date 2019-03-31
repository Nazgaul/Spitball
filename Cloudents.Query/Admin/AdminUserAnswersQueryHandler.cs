using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserAnswersQueryHandler : IQueryHandler<AdminUserAnswersQuery, IEnumerable<UserAnswersDto>>
    {
        private readonly DapperRepository _dapper;
        
        public AdminUserAnswersQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        private const int PageSize = 200;

        public async Task<IEnumerable<UserAnswersDto>> GetAsync(AdminUserAnswersQuery query, CancellationToken token)
        {
            const string sql = @"select A.Id, A.Text, A.Created, A.QuestionId, Q.Text as QuestionText, A.[State]
                from sb.Answer A
                join sb.Question Q
	                on A.QuestionId = Q.Id
                where A.UserId = @Id
                order by 1
                OFFSET @pageSize * @PageNumber ROWS
                FETCH NEXT @pageSize ROWS ONLY;";
            using (var connection = _dapper.OpenConnection())
            {
                return await connection.QueryAsync<UserAnswersDto>(sql,
                    new
                    {
                        id = query.UserId,
                        PageNumber = query.Page,
                        PageSize
                    });
            };
        }
    }
}
