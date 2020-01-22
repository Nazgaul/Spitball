using Cloudents.Core.Entities;
using Cloudents.Query.Users;
using NHibernate;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserDataByIdQueryHandler : IQueryHandler<UserDataByIdQuery, User>
    {
        private readonly IStatelessSession _session;

        public UserDataByIdQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public Task<User> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            return _session.GetAsync<User>(query.Id, token);
        }
    }
}