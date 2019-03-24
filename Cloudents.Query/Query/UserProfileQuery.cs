using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Query
{
    public class UserProfileQuery : IQuery<UserProfileDto>
    {
        public UserProfileQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }


        internal sealed class UserProfileQueryHandler : IQueryHandler<UserProfileQuery, UserProfileDto>
        {
           
            private readonly DapperRepository _session;

            public UserProfileQueryHandler(DapperRepository session)
            {
                _session = session;
            }

            public async Task<UserProfileDto> GetAsync(UserProfileQuery query, CancellationToken token)
            {
        
                    
                using (var conn = _session.OpenConnection())
                {
                    return await conn.QueryFirstOrDefaultAsync<UserProfileDto>(@"
select u.id,u.Image,u.Name,u2.name as universityName, u.Score, u.description, utt.bio, 
case when utt.userrole_id is null then 0 else 1 end as IsTutor
from sb.[user] u 
left join sb.[University] u2 on u.UniversityId2 = u2.Id
left join sb.UserType ut on ut.userid = u.Id
left join sb.UserTutor utt on ut.id = utt.userrole_id
where u.id = @id
and (u.LockoutEnd is null or u.LockoutEnd < GETUTCDATE())
", new { id = query.Id });
                }


            }
        }
    }
}