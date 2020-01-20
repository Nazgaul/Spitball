using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class AdminUserRepository : NHibernateRepository<AdminUser>, IAdminUserRepository
    {
        public AdminUserRepository(ISession session) : base(session)
        {
        }

        public async Task<AdminUser> GetAdminUserByEmailAsync(string email, CancellationToken token)
        {
            return await Session.Query<AdminUser>().FirstOrDefaultAsync(f => f.Email == email, token);
        }
    }
}
