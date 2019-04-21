using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminCoursesQuery : 
            IQuery<IList<PendingCoursesDto>>
    {
        public AdminCoursesQuery(string language, ItemState state)
        {
            Language = language;
            State = state;
        }
        public string Language { get;  }
        public ItemState State { get; }
    }

    internal class AdminPendingCoursesQueryHandler : IQueryHandler<AdminCoursesQuery, IList<PendingCoursesDto>>
    {

        private readonly DapperRepository _dapper;


        public AdminPendingCoursesQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IList<PendingCoursesDto>> GetAsync(AdminCoursesQuery query, CancellationToken token)
        {
            var sql = "select Name from sb.Course where State = @state";

            if (!string.IsNullOrEmpty(query.Language))
            {
                if (query.Language.Equals("he", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " and name like N'%[א-ת]%'";
                }
                else if (query.Language.Equals("en", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " and name like '%[a-z]%'";
                }
            }



            using (var connection = _dapper.OpenConnection())
            {
                var res = await connection.QueryAsync<PendingCoursesDto>(sql, new { state = query.State.ToString() });
                return res.AsList();
            }
        }
    }
}
