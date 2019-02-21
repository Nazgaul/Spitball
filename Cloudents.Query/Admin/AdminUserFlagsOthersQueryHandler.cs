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
    public class AdminUserFlagsOthersQueryHandler : IQueryHandler<AdminUserFlagsOthersQuery, (IEnumerable<UserFlagsOthersDto>, int)>
    {
        private readonly IConfigurationKeys _provider;
        public AdminUserFlagsOthersQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }
        private const int PageSize = 200;
        public async Task<(IEnumerable<UserFlagsOthersDto>, int)> GetAsync(AdminUserFlagsOthersQuery query, CancellationToken token)
        {
            string sql = @"select Id, Country, (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                                (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                                (select count(1) from sb.Answer where FlaggedUserId = U.Id) as flags
                                from sb.[User] U
                                where (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                                (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                                (select count(1) from sb.Answer where FlaggedUserId = U.Id) > @Flags
                                order by 3 desc
                                OFFSET @pageSize * @PageNumber ROWS
                                FETCH NEXT @pageSize ROWS ONLY;";

            if (query.Page == 0)
            {
                sql = sql + @"select count(1) as rows
                            from (
                            select Id, Country, (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                            (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                            (select count(1) from sb.Answer where FlaggedUserId = U.Id) as flags
                            from sb.[User] U
                            where (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                            (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                            (select count(1) from sb.Answer where FlaggedUserId = U.Id) > @Flags
                            ) A;";
            }

            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                using (var res = await connection.QueryMultipleAsync(sql,
                    new
                    {
                        flags = query.MinFlags,
                        PageNumber = query.Page,
                        PageSize
                    })
                    )
                {
                    var resList = res.Read<UserFlagsOthersDto>();
                    int rows = -1;
                    if (!res.IsConsumed)
                    {
                        rows = res.ReadFirst<int>();
                    }
                return (resList, rows);
                }
                // return resList;
            }
        }
    }
}
