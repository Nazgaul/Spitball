using Cloudents.Core.DTOs;
using Cloudents.Infrastructure.Data;
using Cloudents.Query.Query;
using Dapper;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserProfileQueryHandler : IQueryHandler<UserDataByIdQuery, UserProfileDto>
    {
        private readonly DapperRepository _session;

        public UserProfileQueryHandler(DapperRepository session)
        {
            _session = session;
        }

        public async Task<UserProfileDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            /*
            //    if (user.LockoutEnd.HasValue && DateTime.UtcNow < user.LockoutEnd.Value)
            //    {
            //        throw new UserLockoutException();
            //    }
        */
            using (var conn = _session.OpenConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<UserProfileDto>(@"
select u.id,u.Image,u.Name,u2.name as universityName, u.Score
from sb.[user] u join sb.[University] u2 on u.UniversityId2 = u2.Id
where u.id = @id
and (u.LockoutEnd is null or u.LockoutEnd < GETUTCDATE())
", new { id = query.Id });
            }
            //return await _session.Query<User>()
            //     .Fetch(u => u.University)
            //     .Where(w => w.Id == query.Id)
            //     .Select(s => new UserProfileDto
            //     {
            //         Id = s.Id,
            //         Image = s.Image,
            //         Name = s.Name,
            //         UniversityName = s.University.Name,
            //         Score = s.Score
            //     })
            //     .SingleOrDefaultAsync(cancellationToken: token);

        }
    }
}