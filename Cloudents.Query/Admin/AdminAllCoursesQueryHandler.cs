using Cloudents.Core.Interfaces;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminAllCoursesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<string>>
    {
        private readonly DapperRepository _dapper;


        public AdminAllCoursesQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IList<string>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var sql = @"select Name from sb.Course where name like N'%[א-ת]%' and State = 'Ok'";
            var result = await _dapper.WithConnectionAsync(async connection =>
            {
                var res = await connection.QueryAsync<string>(sql);
                return res.AsList();
            }, token);
            return result;
        }
    }
}
