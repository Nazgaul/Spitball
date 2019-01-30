using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Persistance.Repositories
{
   public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public UniversityRepository(ISession session) : base(session)
        {
        }

        public async Task<University> GetUniversityByNameAsync(string name, CancellationToken token)
        {
            return await Session.Query<University>().Where(w => w.Name == name)
                .SingleOrDefaultAsync(token);
        }
    }
}
