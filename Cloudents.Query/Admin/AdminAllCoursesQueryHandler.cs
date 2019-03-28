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
        private readonly IConfigurationKeys _provider;


        public AdminAllCoursesQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }

        public async Task<IList<string>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var sql = @"select Name from sb.Course where name like N'%[א-ת]%' and State = 'Ok'";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                var res = await connection.QueryAsync<string>(sql);
                return res.AsList();
            }
        }
    }
}
