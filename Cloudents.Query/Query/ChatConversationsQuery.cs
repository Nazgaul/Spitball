using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
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

        public long UserId { get; private set; }

        internal sealed class
            GetAnswerAcceptedEmailQueryQueryHandler : IQueryHandler<ChatConversationsQuery, IEnumerable<ChatUserDto>>
        {
            private readonly DapperRepository _dapper;

            public GetAnswerAcceptedEmailQueryQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<IEnumerable<ChatUserDto>> GetAsync(ChatConversationsQuery query, CancellationToken token)
            {

                using (var conn = _dapper.OpenConnection())
                {
                    var result = await conn.QueryAsync< ChatUserDto>(@"Select u.Name,u.Id,u.Image,u.Online,cu2.Unread
 from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
join sb.ChatUser cu2 on cu.ChatRoomId = cr.Id and cu2.Id <> cu.Id
join sb.[User] u on cu2.UserId = u.Id
where cu.UserId = @id", new { id = query.UserId });
                    return result;
                }
               
                //ChatUser ChatUserAlias = null;
                //ChatUser ChatUser2Alias = null;
                //var t = await _session.QueryOver<ChatUser>(() => ChatUserAlias)
                //    .Inner.JoinQueryOver(x => x.ChatRoom)
                //    .Inner.JoinQueryOver(x => x.Users,()=> ChatUser2Alias)
                //    .Where(() => ChatUserAlias.User.Id == query.UserId)
                //    .SelectList(l =>
                //        l.Select(() =>ChatUser2Alias.Id)

                //    )
                //    .ListAsync(token);


                //.Fetch(f => f.User)
                //.Select(s => new ChatUserDto
                //{
                //    Name = s.User.Name,
                //    Id = s.User.Id,
                //    Image = s.User.Image,
                //    Unread = s.Unread,
                //    Online = s.User.Online
                //}).ToListAsync(token);
            }
        }
    }
}