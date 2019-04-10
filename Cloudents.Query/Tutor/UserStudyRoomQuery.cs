using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class UserStudyRoomQuery :IQuery<IEnumerable<UserStudyRoomDto>>
    {
        public UserStudyRoomQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get;  }

        internal sealed class UserStudyRoomQueryHandler : IQueryHandler<UserStudyRoomQuery, IEnumerable<UserStudyRoomDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public UserStudyRoomQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<UserStudyRoomDto>> GetAsync(UserStudyRoomQuery query, CancellationToken token)
            {
                using (var connection = _dapperRepository.OpenConnection())
                {
                    using (var grid = await connection.QueryMultipleAsync(@"Select
 u.Name as Name,
 u.Image as Image,
 u.Online as online,
 sr.Id as id,
 sr.DateTime
from sb.StudyRoom sr 
join sb.StudyRoomUser sru on sr.id = sru.studyRoomId
join sb.[User] u on sr.TutorId = u.Id
where sru.userid = @UserId;

Select
 u.Name as Name,
 u.Image as Image,
 u.Online as online,
 sr.Id as id,
 sr.DateTime
from sb.StudyRoom sr 
join sb.StudyRoomUser sru on sr.id = sru.studyRoomId
join sb.[User] u on sru.UserId = u.Id
where sr.TutorId = @UserId", new { query.UserId }))
                    {
                        var r1 = await grid.ReadAsync<UserStudyRoomDto>();
                        var r2 = await grid.ReadAsync<UserStudyRoomDto>();

                        return r1.Union(r2);
                    }
                }

            }
        }
    }
}