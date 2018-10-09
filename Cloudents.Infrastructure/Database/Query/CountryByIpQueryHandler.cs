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
    public class CountryByIpQueryHandler : IQueryHandler<CountryQuery,string>
    {
        private readonly IStatelessSession _session;

        public CountryByIpQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public Task<string> GetAsync(CountryQuery query, CancellationToken token)
        {
            //nhibernate doesn't support uint
            var address = (long)query.IntAddress;
            return  _session.Query<Ip>().Where(w => w.From < address && address < w.To).Select(s => s.CountryCode)
                .SingleOrDefaultAsync(token);
        }
    }
}