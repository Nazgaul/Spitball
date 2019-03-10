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
    public class AdminNewCoursesQueryHandler : IQueryHandler<AdminEmptyQuery, IList<NewCourseDto>>
    {

        private readonly IConfigurationKeys _provider;


        public AdminNewCoursesQueryHandler(IConfigurationKeys provider)
        {
            _provider = provider;
        }

        public async Task<IList<NewCourseDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var sql = @"with cte as (
                        Select  Name from sb.Course where name like N'%[א-ת]%' and  Created > GETUTCDATE()-15)
                        select cte.Name as NewCourse, c.Name as OldCourse from cte, sb.Course c
                        where (c.Name like '%' + REPLACE(cte.Name,' ','%')  + '%' or cte.Name like '%' + REPLACE(c.Name,' ','%') + '%') 
						and len(c.Name) > 3 and len(cte.Name) > 3 
                        and c.Name <> cte.Name;";
            using (var connection = new SqlConnection(_provider.Db.Db))
            {
                var res = await connection.QueryAsync<NewCourseDto>(sql);
                return res.AsList();
            }
        }
    }
}
