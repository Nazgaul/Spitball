using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminAllUniversitiesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<AllUniversitiesDto>>
    {

        private readonly DapperRepository _dapper;


        public AdminAllUniversitiesQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IList<AllUniversitiesDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var sql = @"select Id,Name from sb.University where name like N'%[א-ת]%' and State = 'Ok'";
            using (var connection = _dapper.OpenConnection())
            {
                var res = await connection.QueryAsync<AllUniversitiesDto>(sql);
                return res.AsList();
            };
             
        }
    }
}
