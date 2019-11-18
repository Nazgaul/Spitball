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
        public AdminCoursesQuery(string language, ItemState state, string country, string filter)
        {
            Language = language;
            State = state;
            Country = country;
            Filter = filter;
        }
        public string Language { get; }
        public ItemState State { get; }
        public string Country { get; }
        public string Filter { get;  }
    }

    internal class AdminPendingCoursesQueryHandler : IQueryHandler<AdminCoursesQuery, IList<PendingCoursesDto>>
    {

        private readonly IDapperRepository _dapper;


        public AdminPendingCoursesQueryHandler(IDapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IList<PendingCoursesDto>> GetAsync(AdminCoursesQuery query, CancellationToken token)
        {
            var sql = @"Select @Term = case when @Term is null then '""""' else '""*' + @Term + '*""' end 
                    select distinct top 100 c.Name
                    from sb.Course c
                        join sb.UsersCourses uc
                        on c.Name = uc.CourseId
                        join sb.[User] u
                        on uc.UserId = u.Id
                    where c.State = @State
                    and (@Term = '""""' or CONTAINS(c.Name, @Term))";

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
                var res = await connection.QueryAsync<PendingCoursesDto>(
                    sql,
                    new { State = query.State.ToString(), query.Country, Term = query.Filter }
                    );
                return res.AsList();
            }
        }
    }
}
