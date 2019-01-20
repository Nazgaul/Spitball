using Cloudents.Infrastructure.Data;
using Cloudents.Query.Query.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminUserIdFromEmailQueryHandler : IQueryHandler<AdminUserIdFromEmailQuery, long>
    {
        private readonly DapperRepository _dapper;

        public AdminUserIdFromEmailQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<long> GetAsync(AdminUserIdFromEmailQuery query, CancellationToken token)
        {
            var result = await _dapper.WithConnectionAsync(async connection =>
            {
                var grid = connection.QueryFirst<long>("select top 1 * from sb.[User] where email = @Email", new { query.Email });
                return grid;
            }, token);
            return result;
        }
    }
}
