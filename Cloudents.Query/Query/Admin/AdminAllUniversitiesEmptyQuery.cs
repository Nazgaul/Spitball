﻿using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminAllUniversitiesEmptyQuery : IQuery<IList<AllUniversitiesDto>>
    {
        internal sealed class AdminAllUniversitiesEmptyQueryHandler : IQueryHandler<AdminAllUniversitiesEmptyQuery, IList<AllUniversitiesDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminAllUniversitiesEmptyQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IList<AllUniversitiesDto>> GetAsync(AdminAllUniversitiesEmptyQuery query, CancellationToken token)
            {
                const string sql = @"select Id,Name from sb.University where name like N'%[א-ת]%' and State = 'Ok'";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<AllUniversitiesDto>(sql);
                    return res.AsList();
                }
            }
        }
    }
}
