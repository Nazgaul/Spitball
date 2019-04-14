using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public class UserDataExpressionQuery : IQuery<RegularUser>
    {
        public UserDataExpressionQuery(Expression<Func<RegularUser, bool>> expression)
        {
            QueryExpression = expression;
        }

        public Expression<Func<RegularUser, bool>> QueryExpression { get; }

        internal sealed class UserDataExpressionQueryHandler : IQueryHandler<UserDataExpressionQuery, RegularUser>
        {
            private readonly IStatelessSession _session;

            public UserDataExpressionQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<RegularUser> GetAsync(UserDataExpressionQuery query, CancellationToken token)
            {
                return await _session.Query<RegularUser>()
                    .Where(query.QueryExpression)
                    .SingleOrDefaultAsync(cancellationToken: token);
            }
        }

    }

    public class UserLoginQuery : IQuery<RegularUser>
    {
        public UserLoginQuery(string loginProvider, string providerKey)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

        public string LoginProvider { get; }
        public string ProviderKey { get; }

        internal sealed class UserLoginQueryHandler : IQueryHandler<UserLoginQuery, RegularUser>
        {
            private readonly IStatelessSession _session;

            public UserLoginQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<RegularUser> GetAsync(UserLoginQuery query, CancellationToken token)
            {
                return await _session.Query<UserLogin>()
                    .Fetch(f => f.User)
                    .Where(w => w.ProviderKey == query.ProviderKey && w.LoginProvider == query.LoginProvider)
                    .Select(s => s.User).SingleOrDefaultAsync(cancellationToken: token);
            }
        }
    }
}