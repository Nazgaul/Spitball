using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserDataByIdQueryHandler : IQueryHandler<UserDataByIdQuery, User>
    {
        private readonly IStatelessSession _session;

        public UserDataByIdQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public Task<User> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            return _session.GetAsync<User>(query.Id, token);
        }
    }
}