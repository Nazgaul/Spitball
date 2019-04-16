using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class StudyRoomQuery : IQuery<StudyRoomDto>
    {
        public StudyRoomQuery(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private Guid Id { get; set; }

        private long UserId { get; }




        internal sealed class StudyRoomQueryHandler : IQueryHandler<StudyRoomQuery, StudyRoomDto>
        {
            private readonly DapperRepository _repository;

            public StudyRoomQueryHandler(DapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<StudyRoomDto> GetAsync(StudyRoomQuery query, CancellationToken token)
            {

                using (var conn = _repository.OpenConnection())
                {
                    return await conn.QuerySingleOrDefaultAsync<StudyRoomDto>(@"
Select srs.sessionId,
onlineDocumentUrl as OnlineDocument, 
cr.Id as ConversationId,
sr.tutorId

from sb.StudyRoom sr join sb.ChatRoom cr on sr.Identifier = cr.Identifier
left join sb.StudyRoomSession srs on sr.Id = srs.StudyRoomId and srs.Ended is null
join sb.StudyRoomUser sru on sr.Id = sru.StudyRoomId and sru.UserId = @UserId
where sr.id = @Id;",
                        new { query.Id, query.UserId });



                }

            }
        }
    }
}