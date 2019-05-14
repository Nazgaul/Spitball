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
with cte as(
select sru.StudyRoomId , u.id,u.PaymentKey,u.PaymentKeyExpiration from sb.StudyRoomUser sru join sb.[User] u on sru.UserId = u.Id 
and (u.PaymentKey is null or u.PaymentKeyExpiration < GetUtcDate())
)

Select 
onlineDocumentUrl as OnlineDocument, 
cr.Id as ConversationId,
sr.tutorId,
 case when tr.Id is null then 1 else 0 end as AllowReview,
 coalesce (
	case when t.price = 0 then 0 else null end,
	case when (select top 1 1 from cte u where sr.Id = u.studyRoomId and u.Id <> sr.TutorId) =1 then 1 else null end,
	0
) as NeedPayment
from sb.StudyRoom sr join sb.ChatRoom cr on sr.Identifier = cr.Identifier
join sb.Tutor t on t.Id = sr.TutorId
join sb.StudyRoomUser sru on sr.Id = sru.StudyRoomId and sru.UserId = @UserId -- to make sure user have permission to enter the room
left join sb.TutorReview tr on sr.Id = tr.RoomId
where sr.id = @Id;",
                        new { query.Id, query.UserId });



                }

            }
        }
    }
}