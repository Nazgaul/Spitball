﻿using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
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
            private readonly DapperRepository _repository;

            public ChatConversationByIdQueryHandler(DapperRepository repository)
            {
                _repository = repository;
            }


            public async Task<IEnumerable<ChatMessageDto>> GetAsync(ChatConversationByIdQuery query, CancellationToken token)
            {
                using (var conn = _repository.OpenConnection())
                {
                    var result = new List<ChatMessageDto>();
                    var reader = await conn.ExecuteReaderAsync(@"
Select
messageType as discriminator, userId,message as Text,creationTime as DateTime , blob as Attachment,
cm.id as id, cr.Id as chatRoomId, u.Image, u.Name
from sb.ChatMessage cm join sb.ChatRoom cr on cm.ChatRoomId = cr.Id
join sb.[user] u
	on cm.UserId = u.Id
where cr.Identifier = @Id
order by cm.Id
OFFSET @PageSize * @PageNumber ROWS 
FETCH NEXT @PageSize ROWS ONLY;", new { Id = query.ConversationId, PageSize = 50, PageNumber = query.Page });

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