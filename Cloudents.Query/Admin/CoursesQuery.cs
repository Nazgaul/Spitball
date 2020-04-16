using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Admin
{
    public class CoursesQuery : IQueryAdmin2<IEnumerable<PendingCoursesDto>>
    {
        public CoursesQuery(string language, ItemState state, Country country, string filter)
        {
            Language = language;
            State = state;
            Country = country;
            Filter = filter;
        }
        public string Language { get; }
        public ItemState State { get; }
        public Country? Country { get; }
        public string Filter { get;  }
    }

    internal class PendingCoursesQueryHandler : IQueryHandler<CoursesQuery, IEnumerable<PendingCoursesDto>>
    {

        private readonly IDapperRepository _dapper;


        public PendingCoursesQueryHandler(IDapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<PendingCoursesDto>> GetAsync(CoursesQuery query, CancellationToken token)
        {
            var sql = @"Select @Term = case when @Term is null then '""""' else '""*' + @Term + '*""' end 
                     select distinct top 100 c.SearchDisplay as Name
                    from 
                    sb.Course2 c
                    join sb.UserCourse2 uc
                    on c.Id = uc.CourseId
                    join sb.[User] u
                    on uc.UserId = u.Id
                    where c.State = @State
                    and (@Term = '""""' or CONTAINS(c.SearchDisplay, @Term))";

            if (query.Country == null)
            {
                sql += " and u.SbCountry = @Country";
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


            using var connection = _dapper.OpenConnection();
            var res = await connection.QueryAsync<PendingCoursesDto>(
                sql,
                new { State = query.State.ToString(), Country = query.Country?.Name, Term = query.Filter }
            );
            return res;
        }
    }
}
