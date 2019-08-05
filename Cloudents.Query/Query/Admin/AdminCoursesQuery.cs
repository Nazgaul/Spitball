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
            IQueryAdmin<IList<PendingCoursesDto>>
    {
        public AdminCoursesQuery(string language, ItemState state, string country)
        {
            Language = language;
            State = state;
            Country = country;
        }
        public string Language { get;  }
        public ItemState State { get; }
        public string Country { get; }
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
            var sql = @"select c.Name 
                    from sb.Course c
                     join sb.UsersCourses uc
                        on c.Name = uc.CourseId
                     join sb.[User] u
                        on uc.UserId = u.Id
                    where c.State = @State";

            if (!string.IsNullOrEmpty(query.Country))
            {
                sql += " and u.Country = @Country";
            }
            if (!string.IsNullOrEmpty(query.Language))
            {
                if (query.Language.Equals("he", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " and c.name like N'%[א-ת]%'";
                }
                else if (query.Language.Equals("en", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " and c.name like '%[a-z]%'";
                }
            }



            using (var connection = _dapper.OpenConnection())
            {
                var res = await connection.QueryAsync<PendingCoursesDto>(sql, new { state = query.State.ToString(), query.Country });
                return res.AsList();
            }
        }
    }
}
