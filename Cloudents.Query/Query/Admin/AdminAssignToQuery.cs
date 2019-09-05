using Cloudents.Core.Entities;
using Cloudents.Query.Stuff;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminAssignToQuery : IQuery<IList<string>>
    {

        internal sealed class AdminAssignToQueryHandler : IQueryHandler<AdminAssignToQuery, IList<string>>
        {
            private readonly IStatelessSession _session;

            public AdminAssignToQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }


            public async Task<IList<string>> GetAsync(AdminAssignToQuery query, CancellationToken token)
            {
                AdminUser adminUser = null;
                User user = null;
                IList<string> res = null;

                var r = _session.QueryOver(() => adminUser)
                    .JoinEntityAlias(() => user, () => adminUser.Email == user.Email, JoinType.InnerJoin)
                    .SelectList(l =>
                    l.Select(() => user.Name).WithAlias(() => res));
                return await r.ListAsync<string>();

            }
        }
    }
}
