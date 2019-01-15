using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistance.Repositories
{
    public class StatsRepository : NHibernateRepository<Stats>, IStatsRepository
    {
        public StatsRepository(ISession session) : base(session)
        {
        }

        public async Task UpdateTableAsync(CancellationToken token)
        {
            await Session.CreateSQLQuery(@"update sb.[HomeStats]
                            set[users] = (select count(1) from sb.[User])
	                        ,[answers] = (select count(1) from sb.Answer)
	                        ,[SBLs] = (select sum(price) from sb.[Transaction] where[Type] = 'Earned')
	                        ,[money] = (select sum(price)/40 from sb.[Transaction] where[Type] = 'Earned')"
                            ).ExecuteUpdateAsync();

        }
    }
}
