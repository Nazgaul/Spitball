using System;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
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

        public Guid ConversationId { get; }
        public int Page { get; }

        internal sealed class ChatConversationByIdQueryHandler : IQueryHandler<ChatConversationByIdQuery, IEnumerable<ChatMessageDto>>
        {
            private readonly IStatelessSession _session;

            public ChatConversationByIdQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<IEnumerable<ChatMessageDto>> GetAsync(ChatConversationByIdQuery query, CancellationToken token)
            {
                return await _session.Query<ChatMessage>()
                      .Where(w => w.ChatRoom.Id == query.ConversationId)
                      .OrderByDescending(o => o.Id)
                      .Take(50)
                      .Skip(50 * query.Page)
                      .Select(s => new ChatMessageDto
                      {
                          UserId = s.User.Id,
                          Text = s.Message
                      })
                      .ToListAsync(token);
            }
        }
    }
}