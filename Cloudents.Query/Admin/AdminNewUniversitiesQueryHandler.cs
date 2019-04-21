using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminNewUniversitiesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<NewUniversitiesDto>>
    {
        private readonly DapperRepository _dapper;
        

        public AdminNewUniversitiesQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IList<NewUniversitiesDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            const string sql = @"with cte as (
                    Select Id, Name from sb.university where name like N'%[א-ת]%' and CreationTime > GETUTCDATE()-90)
                    select cte.Id as [NewId], cte.Name as NewUniversity, c.Id as OldId, c.Name as OldUniversity  from cte, sb.university c
                    where c.Name like REPLACE(cte.Name,' ','%')
                    and c.Name <> cte.Name";
            using (var connection = _dapper.OpenConnection())
            {
                var res = await connection.QueryAsync<NewUniversitiesDto>(sql);
                return res.AsList();
            }
        }
    }
}
