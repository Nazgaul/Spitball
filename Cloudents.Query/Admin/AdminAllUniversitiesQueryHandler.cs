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
    public class AdminAllUniversitiesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<AllUniversitiesDto>>
    {

        private readonly IConfigurationKeys _provider;


        public AdminAllUniversitiesQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }

        public async Task<IList<AllUniversitiesDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var sql = @"select Id,Name from sb.University where name like N'%[א-ת]%' and State = 'Ok'";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                var res = await connection.QueryAsync<AllUniversitiesDto>(sql);
                return res.AsList();
            }
        }
    }
}
