using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Persistance.Repositories
{
    public class ChatRoomRepository :NHibernateRepository<ChatRoom> , IChatRoomRepository
    {
        public ChatRoomRepository(ISession session) : base(session)
        {
        }


        public async Task<ChatRoom> GetChatRoomAsync(IList<long> usersId, CancellationToken token)
        {
            var t =  await Session.Query<ChatUser>()
                .Where(w => usersId.Contains(w.User.Id))
                .GroupBy(g => g.ChatRoom)
                .Where(w => w.Count() == usersId.Count)
                .Select(s => s.Key.Id)
                .SingleOrDefaultAsync(cancellationToken: token);
            if (t == default)
            {
                return null;
            }
            return await Session.LoadAsync<ChatRoom>(t,token);
            //var t =    Session.QueryOver<ChatUser>()
            //    .WhereRestrictionOn(w => w.User.Id).IsIn(usersId.ToArray())
            //    .Select(Projections.Group<ChatUser>(x => x.ChatRoom))
            //    .Where(Restrictions.Eq(Projections.Count<ChatUser>(x => x), usersId.Count))
            //    .Select(s => s.ChatRoom).SingleOrDefault();
        }

    }
}