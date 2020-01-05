using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminChatConversationByIdQuery : IQueryAdmin<IEnumerable<ChatMessageDto>>
    {
        public AdminChatConversationByIdQuery(string conversationId, int page, string country)
        {
            ConversationId = conversationId;
            Page = page;
            Country = country;
        }

        private string ConversationId { get; }
        private int Page { get; }
        public string Country { get; set; }


        internal sealed class AdminChatConversationByIdQueryHandler : IQueryHandler<AdminChatConversationByIdQuery, IEnumerable<ChatMessageDto>>
        {
            private readonly IDapperRepository _repository;

            public AdminChatConversationByIdQueryHandler(IDapperRepository repository)
            {
                _repository = repository;
            }


            public async Task<IEnumerable<ChatMessageDto>> GetAsync(AdminChatConversationByIdQuery query, CancellationToken token)
            {
                using (var conn = _repository.OpenConnection())
                {
                    var result = new List<ChatMessageDto>();
                    var reader = await conn.ExecuteReaderAsync(@"
Select
messageType as discriminator, cm.userId,message as Text,cm.creationTime as DateTime , blob as Attachment,
cm.id as id, cr.Id as chatRoomId, u.ImageName as Image, u.Name,
case when cu.Unread >= ROW_NUMBER() OVER(PARTITION BY cm.userId ORDER BY cm.Id desc) then 1 else 0 end as Unread
from sb.ChatMessage cm 
join sb.ChatRoom cr 
	on cm.ChatRoomId = cr.Id
join sb.[user] u
	on cm.UserId = u.Id
join sb.ChatUser cu
	on cu.ChatRoomId = cr.Id and cu.UserId != cm.UserId
join sb.[user] tu
	on tu.Id = cu.UserId
where cr.Identifier = @Id and (u.Country = @Country or tu.Country = @Country or @Country is null) 
order by cm.Id desc
OFFSET @PageSize * @PageNumber ROWS 
FETCH NEXT @PageSize ROWS ONLY;", new { Id = query.ConversationId, query.Country, PageSize = 50, PageNumber = query.Page });

                    if (reader.Read())
                    {
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
}