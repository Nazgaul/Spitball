using System;
using Cloudents.Core.DTOs.Users;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Chat
{
    public class ChatConversationsQuery : IQuery<IEnumerable<ChatDto>>
    {
        public ChatConversationsQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

        internal sealed class ChatConversationsQueryHandler : IQueryHandler<ChatConversationsQuery, IEnumerable<ChatDto>>
        {
            private readonly IDapperRepository _dapper;

            public ChatConversationsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }


            public async Task<IEnumerable<ChatDto>> GetAsync(ChatConversationsQuery query, CancellationToken token)
            {
                //_session.Query<ChatUser>()
                //    .Fetch(f=>f.ChatRoom)
                //    .ThenFetchMany(x=>x.Users)
                //    .Where(w=>w.User.Id == query.UserId)
                //    .Select(s=> new ChatDto()
                //    {
                //        DateTime = s.ChatRoom.UpdateTime,
                //        ConversationId = s.ChatRoom.Identifier,
                //        LastMessage = _session.Query<ChatMessage>().Where(w=>w.ChatRoom.Id == s.ChatRoom.Id).OrderByDescending(o=>o.Id).Select(s=>s.Id.ToString()).Take(1).Single(),
                //        Users = new List<ChatUserDto>()
                //        {
                //            new ChatUserDto
                //            {

                //            }
                //        }

                //    })



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
u.Online
from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
join sb.ChatUser cu2 on cu2.ChatRoomId = cr.Id and cu2.Id <> cu.Id
join sb.[User] u on cu2.UserId = u.Id
where cu.UserId = @id
order by cr.UpdateTime desc", new {id = query.UserId});

                
                var chatParser = reader.GetRowParser<ChatDto>();
                var chatUserParser = reader.GetRowParser<ChatUserDto>();


                var typeColumnIndex = reader.GetOrdinal("ConversationId");
                var conversation = new Dictionary<string,ChatDto>();
                while (reader.Read())
                {
                    var type = reader.GetString(typeColumnIndex);
                   
                    var user = chatUserParser(reader);

                    if (!conversation.TryGetValue(type, out var v))
                    {
                        var chat = chatParser(reader);
                        v = chat;
                        conversation.Add(v.ConversationId,v);

                    }
                    v.Users.Add(user);
                    //    case ShapeType.Circle:
                    //        shape = circleParser(reader);
                    //        break;
                    //    case ShapeType.Square:
                    //        shape = squareParser(reader);
                    //        break;
                    //    case ShapeType.Triangle:
                    //        shape = triangleParser(reader);
                    //        break;
                    //    default:
                    //        throw new NotImplementedException();
                    //}

                    //shapes.Add(shape);
                }

                return conversation.Values;


//                var result = await conn.QueryAsync<ChatDto>(@"
//Select 
//u.Name,
//u.Id as UserId,
//u.ImageName as Image,
//u.Online,
//cu.Unread,
//cr.Identifier as ConversationId,
//cr.UpdateTime as DateTime,
//(select top 1 cm.Message  from sb.ChatMessage cm where cm.ChatRoomId = cr.Id order by id desc) as lastMessage
//from sb.ChatUser cu
//join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
//join sb.ChatUser cu2 on cu2.ChatRoomId = cr.Id and cu2.Id <> cu.Id
//join sb.[User] u on cu2.UserId = u.Id
//where cu.UserId = @id
//order by cr.UpdateTime desc", new { id = query.UserId });
//                return result;
            }
        }
    }
}