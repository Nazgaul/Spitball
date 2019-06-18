using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class GetUpdatesEmailUsersQuery : IQuery<IEnumerable<UpdateEmailDto>>
    {
        public GetUpdatesEmailUsersQuery()
        {
        }

        internal sealed class GetUpdatesEmailUsersQueryHandler : IQueryHandler<GetUpdatesEmailUsersQuery, IEnumerable<UpdateEmailDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public GetUpdatesEmailUsersQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<UpdateEmailDto>> GetAsync(GetUpdatesEmailUsersQuery query, CancellationToken token)
            {
                const string sql = @"
select u.Id, u.[Name] as UserName, u.Email as 'To'
from sb.Question q
join sb.UsersCourses uc
	on q.CourseId = uc.CourseId
join sb.[User] u
	on uc.UserId = u.Id
where q.Created > getutcdate() - 10
union
select u.Id, u.[Name] as UserName, u.Email as 'To'
from sb.Document q
join sb.UsersCourses uc
	on q.CourseName = uc.CourseId
join sb.[User] u
	on uc.UserId = u.Id
where q.CreationTime > getutcdate() - 10
order by u.Id";

                using (var connection = _dapperRepository.OpenConnection())
                {
                    return await connection.QueryAsync<UpdateEmailDto>(sql);
                }
            }
        }
    }
}
