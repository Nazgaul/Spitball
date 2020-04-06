using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Query.Chat
{
    public class ChatConversationQuery : IQuery<ChatDto>
    {
        public ChatConversationQuery(string id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private string Id { get; }
        private long UserId { get; }

        internal sealed class ChatConversationQueryHandler : IQueryHandler<ChatConversationQuery, ChatDto>
        {
            private readonly IDapperRepository _dapper;
            private readonly IUrlBuilder _urlBuilder;

            public ChatConversationQueryHandler(IDapperRepository dapper, IUrlBuilder urlBuilder)
            {
                _dapper = dapper;
                _urlBuilder = urlBuilder;
            }


            public async Task<ChatDto> GetAsync(ChatConversationQuery query, CancellationToken token)
            {
                using var conn = _dapper.OpenConnection();
                var reader = await conn.ExecuteReaderAsync(@"
Select 
cu.Unread,
cr.Identifier as ConversationId,
cr.UpdateTime as DateTime,
(select top 1 cm.Message  from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) as lastMessage,
u.Id as UserId,
u.Name,
u.ImageName as Image,
u.Online,
from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
left join sb.ChatUser cu2 on cu2.ChatRoomId = cr.Id and cu2.Id <> cu.Id
left join sb.[User] u on cu2.UserId = u.Id
where cu.UserId = @userId  and cr.identifier = @id
order by cr.UpdateTime desc",
                    new { id = query.Id, query.UserId });

                var chatParser = reader.GetRowParser<ChatDto>();
                var chatUserParser = reader.GetRowParser<ChatUserDto>();


                var typeColumnIndex = reader.GetOrdinal("ConversationId");
                var conversation = new Dictionary<string, ChatDto>();
                while (reader.Read())
                {
                    var type = reader.GetString(typeColumnIndex);

                    var user = chatUserParser(reader);

                    if (!conversation.TryGetValue(type, out var v))
                    {
                        var chat = chatParser(reader);
                        v = chat;
                        conversation.Add(v.ConversationId, v);

                    }

                    user.Image = _urlBuilder.BuildUserImageEndpoint(user.UserId, user.Image);
                    v.Users.Add(user);

                }

                return conversation.Values.FirstOrDefault();


            }
        }
    }
}
