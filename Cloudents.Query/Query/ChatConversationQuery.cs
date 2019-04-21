using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Query
{
    public class ChatConversationQuery : IQuery<ChatUserDto>
    {
        public ChatConversationQuery(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }
        public Guid Id { get; set; }
        public long UserId { get; set; }


        internal sealed class
            ChatConversationQueryHandler : IQueryHandler<ChatConversationQuery, ChatUserDto>
        {
            private readonly DapperRepository _dapper;

            public ChatConversationQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<ChatUserDto> GetAsync(ChatConversationQuery query, CancellationToken token)
            {
                using (var conn = _dapper.OpenConnection())
                {
                    return await conn.QueryFirstOrDefaultAsync<ChatUserDto>(@"
Select u.Name,u.Id as UserId,u.Image,u.Online,cu.Unread, cr.Id as ConversationId, cr.UpdateTime as DateTime
 from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
join sb.ChatUser cu2 on cu2.ChatRoomId = cr.Id and cu2.Id <> cu.Id
join sb.[User] u on cu2.UserId = u.Id
where cu.UserId = @UserId and cr.Id = @Id
order by cr.UpdateTime desc", new { id = query.Id, query.UserId });
                }
            }
        }
    }
}
