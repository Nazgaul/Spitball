using System;
using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Chat
{
    public class ChatConversationByIdQuery : IQuery<IEnumerable<ChatMessageDto>>
    {
        public ChatConversationByIdQuery(string conversationId, int page)
        {
            ConversationId = conversationId;
            Page = page;
        }

        private string ConversationId { get; }
        private int Page { get; }

        internal sealed class ChatConversationByIdQueryHandler : IQueryHandler<ChatConversationByIdQuery, IEnumerable<ChatMessageDto>>
        {
            private readonly IDapperRepository _repository;

            public ChatConversationByIdQueryHandler(IDapperRepository repository)
            {
                _repository = repository;
            }


            public async Task<IEnumerable<ChatMessageDto>> GetAsync(ChatConversationByIdQuery query, CancellationToken token)
            {
                using var conn = _repository.OpenConnection();
                var result = new List<ChatMessageDto>();
                Guid.TryParse(query.ConversationId, out var id2);

                using (var reader = await conn.ExecuteReaderAsync(@"
Select
messageType as discriminator,
cm.userId,message as Text,
cm.creationTime as DateTime ,
blob as Attachment,
cm.id as id,
cr.Id as chatRoomId,
u.ImageName as Image,
u.Name,
 
 case when (select max(unread) from sb.chatUser cu where cu.ChatRoomId = cr.Id and cu.UserId != cm.UserId ) >=
     ROW_NUMBER() OVER( ORDER BY cm.Id desc) then 1 else 0 end as Unread
from sb.ChatMessage cm 
join sb.ChatRoom cr 
	on cm.ChatRoomId = cr.Id
join sb.[user] u
	on cm.UserId = u.Id
where cr.Id = @Id2 or cr.Identifier = @Id
order by cm.Id desc
OFFSET @PageSize * @PageNumber ROWS 
FETCH NEXT @PageSize ROWS ONLY;", new { Id = query.ConversationId, Id2 = id2, PageSize = 50, PageNumber = query.Page }))
                {
                    if (!reader.Read()) return result;
                    var toMessage = reader.GetRowParser<ChatMessageDto>(typeof(ChatTextMessageDto));
                    var toAttachment = reader.GetRowParser<ChatMessageDto>(typeof(ChatAttachmentDto));
                    var col = reader.GetOrdinal("discriminator");
                    do
                    {
                        switch (reader.GetString(col))
                        {
                            case "text":
                                result.Add(toMessage(reader));
                                break;
                            case "attachment":
                                result.Add(toAttachment(reader));
                                break;
                        }
                    } while (reader.Read());
                }

                return result;
            }
        }
    }
}