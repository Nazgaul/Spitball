using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Dapper;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Admin
{
    public class CoursesQuery : IQueryAdmin2<IEnumerable<PendingCoursesDto>>
    {
        public CoursesQuery( Country? country, string? term)
        {
          //  State = state;
            Country = country;
            Term = term;
        }
     //   public ItemState State { get; }
        public Country? Country { get; }
        public string? Term { get; }
    }

    internal class PendingCoursesQueryHandler : IQueryHandler<CoursesQuery, IEnumerable<PendingCoursesDto>>
    {

        private readonly IDapperRepository _dapper;


        public PendingCoursesQueryHandler(IDapperRepository dapper)
        {
            _dapper = dapper;
        }

        [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting", Justification = "Using")]
        public async Task<IEnumerable<PendingCoursesDto>> GetAsync(CoursesQuery query, CancellationToken token)
        {
            var sql = @"Select @Term = case when @Term is null then '""""' else '""*' + @Term + '*""' end 
                    select distinct top 100 c.Name
                    from sb.Course c 
                        left join sb.UsersCourses uc
                        on c.Name = uc.CourseId
                        left join sb.[User] u
                        on uc.UserId = u.Id
                    where 
                     (@Term = '""""' or CONTAINS(c.Name, @Term))";

            if (query.Country != null)
            {
                sql += " and u.SbCountry = @Country";
            }

            using var connection = _dapper.OpenConnection();
            return await connection.QueryAsync<PendingCoursesDto>(
                 sql,
                 new
                 {
                     
                     Country = query.Country?.Id,
                     query.Term
                 }
             );
        }
    }
}
