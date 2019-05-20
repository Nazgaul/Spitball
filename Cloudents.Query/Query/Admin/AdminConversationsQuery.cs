using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminConversationsQuery : IQuery<IEnumerable<ConversationDto>>
    {
        internal sealed class AdminAllConversationsQueryHandler : IQueryHandler<AdminConversationsQuery, IEnumerable<ConversationDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminAllConversationsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<ConversationDto>> GetAsync(AdminConversationsQuery query, CancellationToken token)
            {
                const string sql = @"
with cte as (
 select userid,ChatRoomId, u.Name
 from sb.ChatUser cu
 join sb.[user] u
	on u.Id = cu.UserId
)
select cr.identifier as Id,
	(select top 1 cm.CreationTime from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) as lastMessage,
	(select top 1 cte.Name 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId and cm.UserId = cte.UserId
						where cm.ChatRoomId = cr.id 
						order by  cm.CreationTime)  as UserName,
		case when cte.Name not in (select top 1 cte.Name 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId and cm.UserId = cte.UserId
						where cm.ChatRoomId = cr.id 
						order by  cm.CreationTime) then cte.Name end as TutorName
from sb.ChatRoom cr
join cte on cr.Id = cte.ChatRoomId
where cte.Name not in (select top 1 cte.Name 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId and cm.UserId = cte.UserId
						where cm.ChatRoomId = cr.id 
						order by  cm.CreationTime)
	and (select top 1 cte.Name 
						from sb.ChatMessage cm 
						join cte on cm.ChatRoomId = cte.ChatRoomId and cm.UserId = cte.UserId
						where cm.ChatRoomId = cr.id 
						order by  cm.CreationTime) is not null
order by (select top 1 cm.CreationTime from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) desc";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<ConversationDto>(sql);
                    return res;
                }
            }
        }
    }
}
