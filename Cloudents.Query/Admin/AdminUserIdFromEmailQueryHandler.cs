using Cloudents.Query.Query.Admin;
using Dapper;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cloudents.Query.Admin
{
    [UsedImplicitly]
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
                var grid = await connection.QueryFirstOrDefaultAsync<long>("select top 1 * from sb.[User] where email = @Email", new { query.Email });
                return grid;
            }, token);
            return result;
        }
    }
}
