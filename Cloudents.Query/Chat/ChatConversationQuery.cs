using Cloudents.Core.DTOs.Users;
using Dapper;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Chat
{
    public class ChatConversationQuery : IQuery<ChatUserDto>
    {
        public ChatConversationQuery(string id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private string Id { get; }
        private long UserId { get; }


        internal sealed class
            ChatConversationQueryHandler : IQueryHandler<ChatConversationQuery, ChatUserDto>
        {
            private readonly IDapperRepository _dapper;

            public ChatConversationQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<ChatUserDto> GetAsync(ChatConversationQuery query, CancellationToken token)
            {
                using (var conn = _dapper.OpenConnection())
                {
                    return await conn.QueryFirstOrDefaultAsync<ChatUserDto>(@"
Select u.Name,u.Id as UserId,u.ImageName as Image,u.Online,cu.Unread, cr.Identifier as ConversationId, cr.UpdateTime as DateTime
 from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
join sb.ChatUser cu2 on cu2.ChatRoomId = cr.Id and cu2.Id <> cu.Id
join sb.[User] u on cu2.UserId = u.Id
where cu.UserId = @UserId and cr.Identifier = @Id
order by cr.UpdateTime desc", new { id = query.Id, query.UserId });
                }
            }
        }
    }
}
