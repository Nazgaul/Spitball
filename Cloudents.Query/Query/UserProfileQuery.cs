using System.Linq;
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
                    
                    var t =  await conn.QueryAsync<UserProfileDto, UserTutorProfileDto, UserProfileDto>(@"
select u.id,u.Image,u.Name,u2.name as universityName, u.Score, u.description,
t.price as price, u.FirstName,u.LastName,
u.online,
(Select avg(rate) from sb.tutorReview where tutorId = t.Id) as rate,
(Select count(*) from sb.tutorReview where tutorId = t.Id) as ReviewCount
from sb.[user] u 
left join sb.[University] u2 on u.UniversityId2 = u2.Id
left join sb.Tutor t
	on U.Id = t.Id
where u.id = @Id
and (u.LockoutEnd is null or u.LockoutEnd < GETUTCDATE())
", (dto, profileDto) =>
                    {
                        dto.Tutor = profileDto;
                        return dto;
                    } , new { id = query.Id },splitOn:"price");
                    return t.FirstOrDefault();
                }


            }
        }
    }
}