﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class UserLoginQueryHandler : IQueryHandler<UserLoginQuery, User>
    {
        private readonly IStatelessSession _session;

        public UserLoginQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public Task<User> GetAsync(UserLoginQuery query, CancellationToken token)
        {
            return _session.Query<UserLogin>()
                .Fetch(f => f.User)
                .Where(w => w.ProviderKey == query.ProviderKey && w.LoginProvider == query.LoginProvider)
                .Select(s => s.User).SingleOrDefaultAsync(cancellationToken: token);
        }
    }
}