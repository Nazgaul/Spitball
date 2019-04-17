﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class UserStudyRoomQuery : IQuery<IEnumerable<UserStudyRoomDto>>
    {
        public UserStudyRoomQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

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
                    var result = await connection.QueryAsync<UserStudyRoomDto>(@"Select
 u.Name as Name,
 u.Image as Image,
 u.Online as online,
 sr.Id as id,
 sr.DateTime,
 (select id from sb.ChatRoom where Identifier = sr.Identifier) as conversationId
from sb.StudyRoom sr
join sb.StudyRoomUser sru on sr.id = sru.studyRoomId
join sb.[User] u on sru.UserId = u.Id
where sr.Id in (select StudyRoomId from sb.StudyRoomUser where userid = @UserId)
and sru.UserId <> @UserId", new { query.UserId });
                    return result;

                }

            }
        }
    }
}