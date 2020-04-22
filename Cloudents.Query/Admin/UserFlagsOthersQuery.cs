using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class UserFlagsOthersQuery : IQueryAdmin<(IEnumerable<UserFlagsOthersDto>, int)>
    {
        public UserFlagsOthersQuery(int minFlags, int page, string? country)
        {
            Page = page;
            MinFlags = minFlags;
            Country = country;
        }
        private int Page { get; }
        private int MinFlags { get; }
        public string? Country { get; }
        internal sealed class UserFlagsOthersQueryHandler : IQueryHandler<UserFlagsOthersQuery, (IEnumerable<UserFlagsOthersDto>, int)>
        {
            private readonly IDapperRepository _dapper;
            public UserFlagsOthersQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }
            private const int PageSize = 200;

            public async Task<(IEnumerable<UserFlagsOthersDto>, int)> GetAsync(UserFlagsOthersQuery query, CancellationToken token)
            {
                var sql = @"select Id, Country, (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                                (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                                (select count(1) from sb.Answer where FlaggedUserId = U.Id) as flags
                                from sb.[User] U
                                where (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                                (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                                (select count(1) from sb.Answer where FlaggedUserId = U.Id) > @Flags
                                    and (U.Country = @Country or @Country is null)
                                order by 3 desc
                                OFFSET @pageSize * @PageNumber ROWS
                                FETCH NEXT @pageSize ROWS ONLY;";

                if (query.Page == 0)
                {
                    sql += @"select count(1) as rows
                            from (
                            select Id, Country, (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                            (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                            (select count(1) from sb.Answer where FlaggedUserId = U.Id) as flags
                            from sb.[User] U
                            where (select count(1) from sb.Document where FlaggedUserId = U.Id) + 
			                            (select count(1) from sb.Question where FlaggedUserId = U.Id) +
			                            (select count(1) from sb.Answer where FlaggedUserId = U.Id) > @Flags
                                and (U.Country = @Country or @Country is null)
                            ) A;";
                }

                using var connection = _dapper.OpenConnection();
                using var res = await connection.QueryMultipleAsync(sql,
                    new
                    {
                        flags = query.MinFlags,
                        PageNumber = query.Page,
                        PageSize,
                        query.Country
                    });
                var resList = res.Read<UserFlagsOthersDto>();
                var rows = -1;
                if (!res.IsConsumed)
                {
                    rows = res.ReadFirst<int>();
                }
                return (resList, rows);
            }
        }
    }
}
