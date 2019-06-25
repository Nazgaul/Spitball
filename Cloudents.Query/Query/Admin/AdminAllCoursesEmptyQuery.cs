using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminAllCoursesEmptyQuery : IQuery<IList<string>>
    {
        internal sealed class AdminAllCoursesEmptyQueryHandler : IQueryHandler<AdminAllCoursesEmptyQuery, IList<string>>
        {
            private readonly DapperRepository _dapper;


            public AdminAllCoursesEmptyQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IList<string>> GetAsync(AdminAllCoursesEmptyQuery query, CancellationToken token)
            {
                const string sql = @"select Name from sb.Course where name like N'%[א-ת]%' and State = 'Ok'";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<string>(sql);
                    return res.AsList();
                }
            }
        }
    }
}
