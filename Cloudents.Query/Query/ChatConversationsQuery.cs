using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class ChatConversationsQuery : IQuery<IEnumerable<ChatUserDto>>
    {
        public ChatConversationsQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; private set; }

        internal sealed class
            GetAnswerAcceptedEmailQueryQueryHandler : IQueryHandler<ChatConversationsQuery, IEnumerable<ChatUserDto>>
        {
            private readonly DapperRepository _dapper;

            public GetAnswerAcceptedEmailQueryQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<IEnumerable<ChatUserDto>> GetAsync(ChatConversationsQuery query, CancellationToken token)
            {
                using (var conn = _dapper.OpenConnection())
                {
                    var result = await conn.QueryAsync< ChatUserDto>(@"
Select u.Name,u.Id as UserId,u.Image,u.Online,cu.Unread, cr.Id as ConversationId, cr.UpdateTime as DateTime
 from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
join sb.ChatUser cu2 on cu2.ChatRoomId = cr.Id and cu2.Id <> cu.Id
join sb.[User] u on cu2.UserId = u.Id
where cu.UserId = @id
order by cr.UpdateTime desc", new { id = query.UserId });
                    return result;
                }
            }
        }
    }
}