﻿using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
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

        private long UserId { get;  }

        internal sealed class ChatConversationsQueryHandler : IQueryHandler<ChatConversationsQuery, IEnumerable<ChatUserDto>>
        {
            private readonly DapperRepository _dapper;

            public ChatConversationsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<IEnumerable<ChatUserDto>> GetAsync(ChatConversationsQuery query, CancellationToken token)
            {
                using (var conn = _dapper.OpenConnection())
                {
                    var result = await conn.QueryAsync<ChatUserDto>(@"
Select u.Name,
u.Id as UserId,
u.Image,
u.Online,
cu.Unread,
cr.Id as ConversationId,
cr.UpdateTime as DateTime,
(select id from sb.StudyRoom where Identifier = cr.identifier)  as StudyRoomId,
(select top 1 cm.Message  from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) as lastMessage
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