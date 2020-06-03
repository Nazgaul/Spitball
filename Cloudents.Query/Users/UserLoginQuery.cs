using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Users
{
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
            public Task<User> GetAsync(UserLoginQuery query, CancellationToken token)
            {
                return _session.Query<UserLogin>()
                    .WithOptions(w => w.SetComment(nameof(UserLoginQuery)))
                    .Fetch(f => f.User)
                    .Where(w => w.ProviderKey == query.ProviderKey && w.LoginProvider == query.LoginProvider)
                    .Select(s => s.User).SingleOrDefaultAsync(cancellationToken: token);
            }
        }
    }
}