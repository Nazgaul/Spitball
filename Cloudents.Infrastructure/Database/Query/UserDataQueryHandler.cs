﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserDataQueryHandler : IQueryHandler<UserDataExpressionQuery, RegularUser>
    {
        private readonly IStatelessSession _session;

        public UserDataQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public  Task<RegularUser> GetAsync(UserDataExpressionQuery query, CancellationToken token)
        {
            return _session.Query<RegularUser>().FirstOrDefaultAsync(query.QueryExpression, token);
        }
    }
}