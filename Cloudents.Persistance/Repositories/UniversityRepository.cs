using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistance.Repositories
{
    public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public UniversityRepository(ISession session) : base(session)
        {
        }

        public async Task<University> GetUniversityByNameAsync(string name, CancellationToken token)
        {
            return await Session.Query<University>()

                .Where(w => w.Name == name)
                // ReSharper disable once UseCollectionCountProperty nhibernate will be mad - need to use it because of readonly collection
                .OrderByDescending(o =>o.Documents.Count())
                .FirstOrDefaultAsync(token);
            //.SingleOrDefaultAsync(token);
        }
    }
}
