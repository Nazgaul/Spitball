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
    public class AdminPendingCoursesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<PendingCoursesDto>>
    {

        private readonly IConfigurationKeys _provider;


        public AdminPendingCoursesQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }

        public async Task<IList<PendingCoursesDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var sql = @"select Name, Created from sb.Course where State = 'Pending'";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                var res = await connection.QueryAsync<PendingCoursesDto>(sql);
                return res.AsList();
            }
        }
    }
}
