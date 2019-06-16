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
                const string sql = @"
with cte as (
Select 
cr.Id , 
cr.status,
cr.Identifier ,
cr.UpdateTime as lastMessage,
u.id as userId,
u.Name,
u.Email,
u.PhoneNumberHash,
case when (select top 1 UserId from sb.ChatMessage cm where  cm.ChatRoomId = cr.id ) = cu.userid then 0 else 1 end as isTutor

from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
join sb.[user] u on cu.UserId = u.Id
)
select c.Identifier as id,
c.lastMessage as lastMessage,
c.Name as UserName,
c.PhoneNumberHash as UserPhoneNumber,
c.Email as UserEmail,
d.Name as TutorName,
d.PhoneNumberHash as TutorPhoneNumber,
d.Email as TutorEmail,
c.status,

(SELECT max (grp) FROM 
(
SELECT *, COUNT(isstart) OVER( PARTITION BY ChatRoomId ORDER BY Id ROWS UNBOUNDED PRECEDING) AS grp
FROM (
SELECT *,
CASE WHEN ABS(UserId - LAG(UserId) OVER(PARTITION BY ChatRoomId ORDER BY Id)) <= 1 THEN NULL ELSE 1 END AS isstart
FROM sb.ChatMessage
where ChatRoomId = c.id
) t1
) t2) as conversationStatus
 
from cte c inner join cte d on d.id = c.id and c.isTutor = 0 and d.isTutor = 1
 where (c.UserId = @UserId or @UserId = 0 or d.userId = @UserId)
order by c.lastMessage desc";
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
