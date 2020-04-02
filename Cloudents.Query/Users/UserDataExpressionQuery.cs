﻿using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserDataExpressionQuery : IQuery<User>
    {
        public UserDataExpressionQuery(Expression<Func<User, bool>> expression)
        {
            QueryExpression = expression;
        }

        private Expression<Func<User, bool>> QueryExpression { get; }

        internal sealed class UserDataExpressionQueryHandler : IQueryHandler<UserDataExpressionQuery, User>
        {
            private readonly IStatelessSession _session;

            public UserDataExpressionQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<User> GetAsync(UserDataExpressionQuery query, CancellationToken token)
            {
                return await _session.Query<User>()
                    .WithOptions(w => w.SetComment(nameof(UserDataExpressionQuery)))
                    .Where(query.QueryExpression)
                    .SingleOrDefaultAsync(token);
            }
        }

    }

    public class UserLoginQuery : IQuery<User>
    {
        public UserLoginQuery(string loginProvider, string providerKey)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

        private string LoginProvider { get; }
        private string ProviderKey { get; }

        internal sealed class UserLoginQueryHandler : IQueryHandler<UserLoginQuery, User>
        {
            private readonly IStatelessSession _session;

            public UserLoginQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<User> GetAsync(UserLoginQuery query, CancellationToken token)
            {
                return await _session.Query<UserLogin>()
                    .WithOptions(w => w.SetComment(nameof(UserLoginQuery)))
                    .Fetch(f => f.User)
                    .Where(w => w.ProviderKey == query.ProviderKey && w.LoginProvider == query.LoginProvider)
                    .Select(s => s.User).SingleOrDefaultAsync(cancellationToken: token);
            }
        }
    }
}