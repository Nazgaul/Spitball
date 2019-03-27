using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class ChatConversationByIdQuery : IQuery<IEnumerable<ChatMessageDto>>
    {
        public ChatConversationByIdQuery(Guid conversationId, int page)
        {
            ConversationId = conversationId;
            Page = page;
        }

        private Guid ConversationId { get; }
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
                    var result = await conn.QueryAsync<ChatMessageDto>(@"Select
userId,message as Text,creationTime as DateTime from sb.ChatMessage cm
where ChatRoomId = @Id
order by cm.Id
OFFSET @PageSize * @PageNumber ROWS 
 FETCH NEXT @PageSize ROWS ONLY;", new { Id = query.ConversationId, PageSize = 50, PageNumber = query.Page });
                    return result;
                }
                //return await _session.Query<ChatMessage>()
                //      .Where(w => w.ChatRoom.Id == query.ConversationId)
                //      .OrderBy(o => o.Id)
                //      .Take(50)
                //      .Skip(50 * query.Page)
                //      .Select(s => new ChatMessageDto
                //      {
                //          UserId = s.User.Id,
                //          Text = s.Message,
                //          DateTime = s.CreationTime
                //      })
                //      .ToListAsync(token);
            }
        }
    }
}