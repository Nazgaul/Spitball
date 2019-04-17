using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Persistence.Repositories
{
    public class ChatRoomRepository :NHibernateRepository<ChatRoom> , IChatRoomRepository
    {
        public ChatRoomRepository(ISession session) : base(session)
        {
        }


        public Task<ChatRoom> GetChatRoomAsync(IEnumerable<long> usersId, CancellationToken token)
        {
            var identifier = ChatRoom.BuildChatRoomIdentifier(usersId);
            return GetChatRoomAsync(identifier, token);
        }

        public async Task<ChatRoom> GetChatRoomAsync(string identifier, CancellationToken token)
        {
            return await Session.Query<ChatRoom>()
                .Where(t => t.Identifier == identifier).SingleOrDefaultAsync(token);
        }

    }
}