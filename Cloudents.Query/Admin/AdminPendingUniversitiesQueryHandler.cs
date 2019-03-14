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
    public class AdminPendingUniversitiesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<PendingUniversitiesDto>>
    {

        private readonly IConfigurationKeys _provider;


    public AdminPendingUniversitiesQueryHandler(IConfigurationKeys provider)
    {
        _provider = provider;
    }

    public async Task<IList<PendingUniversitiesDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
    {
        var sql = @"select Id, Name, CreationTime as Created from sb.University where State = 'Pending'";
        using (var connection = new SqlConnection(_provider.Db.Db))
        {
            var res = await connection.QueryAsync<PendingUniversitiesDto>(sql);
            return res.AsList();
        }
    }
}
}
