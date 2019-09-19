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
            private readonly IDapperRepository _repository;

            public StudyRoomQueryHandler(IDapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<StudyRoomDto> GetAsync(StudyRoomQuery query, CancellationToken token)
            {
                //TODO: make it better
                using (var conn = _repository.OpenConnection())
                {
                    return await conn.QuerySingleOrDefaultAsync<StudyRoomDto>(@"
Select 
onlineDocumentUrl as OnlineDocument, 
sr.identifier as ConversationId,
sr.tutorId,
u.Name as TutorName,
u.Image as TutorImage,
u1.Id as StudentId, u1.Name as StudentName, u1.Image as StudentImage,

 coalesce (
	case when t.price = 0 then 0 else null end,
	case when u1.PaymentExists = 1 then 0 else null end,
    case when u1.Country != 'IL' then 0 else null end,
	1
) as NeedPayment
from sb.StudyRoom sr 
join sb.Tutor t on t.Id = sr.TutorId
join sb.[User] u on t.Id = u.Id
join sb.StudyRoomUser sru1 on sr.Id = sru1.StudyRoomId and sru1.UserId != sr.TutorId
join sb.StudyRoomUser sru2 on sr.Id = sru2.StudyRoomId and sru2.UserId = @UserId
join sb.[user] u1 on sru1.UserId = u1.Id
where sr.id = @Id;",
                        new { query.Id, query.UserId });



                }

            }
        }
    }
}