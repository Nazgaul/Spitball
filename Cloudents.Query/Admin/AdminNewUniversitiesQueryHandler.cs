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
    public class AdminNewUniversitiesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<NewUniversitiesDto>>
    {

        private readonly IConfigurationKeys _provider;


        public AdminNewUniversitiesQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }

        public async Task<IList<NewUniversitiesDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var sql = @"with cte as (
                Select Id, Name from sb.university where name like N'%[א-ת]%' and CreationTime > GETUTCDATE()-90)
                select cte.Id as [NewId], cte.Name as NewUniversity, c.Id as OldId, c.Name as OldUniversity  from cte, sb.university c
                where (c.Name like '%' + REPLACE(cte.Name,' ','%') + '%' or cte.Name like '%' +  REPLACE(c.Name,' ','%') + '%')
                and c.Name <> cte.Name;";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                var res = await connection.QueryAsync<NewUniversitiesDto>(sql);
                return res.AsList();
            }
        }
    }
}
