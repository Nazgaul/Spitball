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
    public class AdminUserFlagsQueryHandler : IQueryHandler<AdminUserFlagsQuery, IEnumerable<UserFlagsDto>>
    {
        private readonly DapperRepository _dapper;

        public AdminUserFlagsQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }
        private const int PageSize = 200;

        public async Task<IEnumerable<UserFlagsDto>> GetAsync(AdminUserFlagsQuery query, CancellationToken token)
        {
            const string sql = @"select A.Text, A.Created, A.State, A.FlagReason, A.VoteCount, 'Answer' as ItemType
                                from sb.Answer A
                                where FlaggedUserId = @Id
                                union
                                select Q.Text, Q.Created, Q.State, Q.FlagReason, Q.VoteCount, 'Question' as ItemType
                                from sb.Question Q
                                where FlaggedUserId = @Id
                                union
                                select D.Name, D.CreationTime, D.State, D.FlagReason, D.VoteCount, 'Document' as ItemType
                                from sb.Document D
                                where FlaggedUserId = @Id
                                order by 2 desc
                                OFFSET @pageSize * @PageNumber ROWS
                                FETCH NEXT @pageSize ROWS ONLY;";
            using (var connection = _dapper.OpenConnection())
            {
                return await connection.QueryAsync<UserFlagsDto>(sql,
                    new
                    {
                        id = query.Id,
                        PageNumber = query.Page,
                        PageSize
                    });
            };
        }
    }
}
