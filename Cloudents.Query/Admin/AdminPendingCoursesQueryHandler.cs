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
    public class AdminPendingCoursesQueryHandler : IQueryHandler<AdminLanguageQuery, IList<PendingCoursesDto>>
    {

        private readonly IConfigurationKeys _provider;


        public AdminPendingCoursesQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }

        public async Task<IList<PendingCoursesDto>> GetAsync(AdminLanguageQuery query, CancellationToken token)
        {
            var sql = @"select Name from sb.Course where State = 'Pending'";
            if (query.Language == "he")
            {
                sql += "and name like N'%[א-ת]%'";
            }
            else if (query.Language == "en")
            {
                sql += "and name like '%[a-z]%'";
            }
            
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                var res = await connection.QueryAsync<PendingCoursesDto>(sql);
                return res.AsList();
            }
        }
    }
}
