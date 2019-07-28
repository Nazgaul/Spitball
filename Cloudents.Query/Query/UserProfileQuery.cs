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
           
            private readonly IDapperRepository _session;

            public UserProfileQueryHandler(IDapperRepository session)
            {
                _session = session;
            }

            public async Task<UserProfileDto> GetAsync(UserProfileQuery query, CancellationToken token)
            {
        
                    
                using (var conn = _session.OpenConnection())
                {
                    //TODO : change to nhibernate
                    
                    var t =  await conn.QueryAsync<UserProfileDto, UserTutorProfileDto, UserProfileDto>(@"
select u.id,u.Image,u.Name,u2.name as universityName, u.Score, u.description,u.online,
t.price as price, u.FirstName,u.LastName,

(Select avg(rate) from sb.tutorReview where tutorId = t.Id) as rate,
(Select count(*) from sb.tutorReview where tutorId = t.Id) as ReviewCount
from sb.[user] u 
left join sb.[University] u2 on u.UniversityId2 = u2.Id
left join sb.Tutor t 
	on U.Id = t.Id and t.State = 'Ok'
where u.id = @Id
and (u.LockoutEnd is null or u.LockoutEnd < GetUtcDate())
", (dto, profileDto) =>
                    {
                        dto.Tutor = profileDto;
                        return dto;
                    } , new { id = query.Id },splitOn:"price");
                    return t.SingleOrDefault();
                }


            }
        }
    }
}