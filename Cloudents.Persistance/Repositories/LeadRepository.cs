using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class LeadRepository : NHibernateRepository<Lead>, ILeadRepository
    {
        public LeadRepository(ISession session) : base(session)
        {
        }

        public async Task<bool> NeedToSendMoreTutorsAsync(long userId, CancellationToken token)
        {
            var dateTime = DateTime.UtcNow.AddDays(-1);
            var futureLeadCount = Session.QueryOver<Lead>()

                .Where(w => w.User.Id == userId)
                .Where(w => w.CreationTime > dateTime)
                .ToRowCountQuery()
                .FutureValue<int>();

            ChatRoomAdmin chatRoomAdminAlias = null;
            Lead leadAlias = null;
            var futureChatCount = Session.QueryOver(() => leadAlias)
                .JoinEntityAlias(() => chatRoomAdminAlias, () => leadAlias.Id == chatRoomAdminAlias.Lead.Id)
                .Where(w => w.User.Id == userId)
                .Where(w => w.CreationTime > dateTime)
                .ToRowCountQuery()
                .FutureValue<int>();
            var leadCount = await futureLeadCount.GetValueAsync(token);

            var chatCount = await futureChatCount.GetValueAsync(token);

            return leadCount == chatCount;

        }
    }
}
