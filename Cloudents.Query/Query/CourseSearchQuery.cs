using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Dapper;


namespace Cloudents.Query.Query
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(string term)
        {
            Term = term;
        }

        public string Term { get; }


       
        internal sealed class CoursesByTermQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public CoursesByTermQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
           {
                var sql = @"select top 10 Name
                            from sb.Course
                            where CONTAINS(Name, @Term) and State = 'OK'
                            order by [Count] desc";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new { Term = query.Term});
                }
                    /*return await _session.Query<Course>()
                    //.Where(w => w.Name.IsLike(query.Term,MatchMode.End))
                    .Where(w => w.Id.Contains(query.Term) && w.State == ItemState.Ok)
                    .OrderByDescending(o => o.Count)
                    .Take(10).Select(s => new CourseDto(s.Id)).ToListAsync(token);*/

            }
        }
    }
    
}