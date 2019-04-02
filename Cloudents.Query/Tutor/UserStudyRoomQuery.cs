using System.Collections.Generic;
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
                  return  await connection.QueryAsync<UserStudyRoomDto>(
                        @"Select u.Name as Name, u.Image as Image, u.Online as online, sr.Id as id, sr.DateTime
from sb.StudyRoom sr 
join sb.StudyRoomUser sru on sr.id = sru.studyroomid
join sb.[User] u on sr.TutorId = u.Id
where sru.userid = @UserId", new {query.UserId});
                }

            }
        }
    }
}