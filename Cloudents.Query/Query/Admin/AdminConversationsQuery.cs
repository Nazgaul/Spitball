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
 select userid,ChatRoomId, u.Name,
 case when u.Id in (select Id from sb.Tutor where Id = u.Id) then 1 else 0 end as IsTotur,
  ROW_NUMBER() over (partition by ChatRoomId order by userid) as n 
 from sb.ChatUser cu
 join sb.[user] u
	on u.Id = cu.UserId
)
select cr.Id,
	(select top 1 cm.CreationTime from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) as lastMessage,
	(select [Name] from cte where ChatRoomId = cr.Id and n = 1) as UserName1,
	(select IsTotur from cte where ChatRoomId = cr.Id and n = 1) as IsTotur1,
	(select [Name] from cte where ChatRoomId = cr.Id and n = 2) as UserName2,
	(select IsTotur from cte where ChatRoomId = cr.Id and n = 2) as IsTotur2
from sb.ChatRoom cr";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<ConversationDto>(sql);
                    return res;
                }
            }
        }
    }
}
