using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class GetRandomFictiveUserIdQueryHandler : IQueryHandler<AdminEmptyQuery, long>
    {
        private readonly IStatelessSession _session;

        public GetRandomFictiveUserIdQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public Task<long> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return _session.QueryOver<User>().Where(w => w.Fictive)
                .OrderByRandom()
                .Select(s=> s.Id)
                .Take(1)
                .SingleOrDefaultAsync<long>(token);
        }
    }
}