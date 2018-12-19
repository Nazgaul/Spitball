using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;
using Cloudents.Domain.Entities;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserDataByIdQueryHandler : IQueryHandler<UserDataByIdQuery, RegularUser>
    {
        private readonly IStatelessSession _session;

        public UserDataByIdQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public Task<RegularUser> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            return _session.GetAsync<RegularUser>(query.Id, token);
        }
    }
}