using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class AdminLanguageRepository : NHibernateRepository<AdminLanguage>, IAdminLanguageRepository
    {
        public AdminLanguageRepository(ISession session) : base(session)
        {
        }

        public async Task<AdminLanguage> GetLanguageByNameAsync(string name, CancellationToken token)
        {
            return await Session.Query<AdminLanguage>()
                .Where(w => w.Name == name)
                .FirstOrDefaultAsync(token);
        }
    }
}
