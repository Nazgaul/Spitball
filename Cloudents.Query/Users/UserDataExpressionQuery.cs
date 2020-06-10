using Cloudents.Core.Entities;
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

            public UserDataExpressionQueryHandler(IStatelessSession session)
            {
                _session = session;
            }
            public Task<User> GetAsync(UserDataExpressionQuery query, CancellationToken token)
            {
                return _session.Query<User>().Fetch(f=>f.Tutor)
                    .WithOptions(w => w.SetComment(nameof(UserDataExpressionQuery)))
                    .Where(query.QueryExpression)
                    .SingleOrDefaultAsync(token);
            }
        }

    }
}