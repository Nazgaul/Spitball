using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminConversationsQuery : IQuery<IEnumerable<ConversationDto>>
    {
        public AdminConversationsQuery(long userId)
        {
            UserId = userId;
        }
        private long UserId { get; set; }
        internal sealed class AdminAllConversationsQueryHandler : IQueryHandler<AdminConversationsQuery, IEnumerable<ConversationDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminAllConversationsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<ConversationDto>> GetAsync(AdminConversationsQuery query, CancellationToken token)
            {
                //TODO: make this quey better
                const string sql = @"
with cte as (
 select userid,ChatRoomId, u.Name, PhoneNumberHash, Email
 from sb.ChatUser cu
 join sb.[user] u
	on u.Id = cu.UserId
)

select cr.identifier as Id,
	(select top 1 cm.CreationTime from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) as lastMessage,
	(select top 1 cte.Name 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId
						where cm.ChatRoomId = cr.id and cm.UserId = cte.UserId
						order by cm.CreationTime) as UserName,
	(select top 1 cte.PhoneNumberHash 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId
						where cm.ChatRoomId = cr.id and cm.UserId = cte.UserId
						order by cm.CreationTime) as UserPhoneNumber,
	(select top 1 cte.Email 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId
						where cm.ChatRoomId = cr.id and cm.UserId = cte.UserId
						order by cm.CreationTime) as UserEmail,
						c2.Name as TutorName,
						c2.PhoneNumberHash as TutorPhoneNumber,
						c2.Email as TutorEmail,
	case when (select count(distinct UserId) from sb.ChatMessage cm where cm.ChatRoomId = cr.id) = 1 then 'Student'
	when (select count(distinct UserId) from sb.ChatMessage cm where cm.ChatRoomId = cr.id) = 2
		and (select count(1) from sb.ChatMessage cm where cm.ChatRoomId = cr.id) = 2 then 'Tuter'
	else 'Conversation' end as [Status]
from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id and (cu.UserId = @UserId or @UserId = 0)
join cte on cr.Id = cte.ChatRoomId and cu.UserId = cte.UserId
join cte c2 on c2.ChatRoomId = cr.Id and cu.UserId = cte.UserId and c2.UserId != cte.UserId
where c2.Name != (select top 1 cte.Name 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId
						where cm.ChatRoomId = cr.id and cm.UserId = cte.UserId
						order by cm.CreationTime) 
order by (select top 1 cm.CreationTime from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) desc";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<ConversationDto>(sql,
                        new { query.UserId });
                    return res;
                }
            }
        }
    }
}
