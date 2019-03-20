using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

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
            private readonly IStatelessSession _session;

            public GetAnswerAcceptedEmailQueryQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<ChatUserDto>> GetAsync(ChatConversationsQuery query, CancellationToken token)
            {
                return await _session.Query<ChatUser>()
                    .Fetch(f => f.User)
                    .Select(s => new ChatUserDto
                    {
                        Name = s.User.Name,
                        Id = s.User.Id,
                        Image = s.User.Image,
                        Unread = s.Unread,
                        Online = s.User.Online
                    }).ToListAsync(token);
            }
        }
    }
}