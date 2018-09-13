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
    public class UserLoginQueryHandler : IQueryHandler<UserLoginQuery, User>
    {
        private readonly IStatelessSession _session;

        public UserLoginQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
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