using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserDataQueryHandler : IQueryHandler<UserDataExpressionQuery, User>
    {
        private readonly IStatelessSession _session;

        public UserDataQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public  Task<User> GetAsync(UserDataExpressionQuery query, CancellationToken token)
        {
            return _session.Query<User>().FirstOrDefaultAsync(query.QueryExpression, token);
        }
    }
}