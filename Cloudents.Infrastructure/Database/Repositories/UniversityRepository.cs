using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Repositories
{
   public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public UniversityRepository(ISession session) : base(session)
        {
        }

        public async Task<University> GetUniversityByNameAsync(string name,string country, CancellationToken token)
        {
            return await Session.Query<University>().Where(w => w.Name == name && w.Country == country)
                .SingleOrDefaultAsync(token);
        }
    }
}
