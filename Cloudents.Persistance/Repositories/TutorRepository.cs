using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using NHibernate.Linq;

namespace Cloudents.Persistence.Repositories
{
    public class TutorRepository : NHibernateRepository<Tutor>, ITutorRepository
    {
        public TutorRepository(ISession session) : base(session)
        {
        }

        public override Task DeleteAsync(Tutor entity, CancellationToken token)
        {
            entity.Delete();
            return base.DeleteAsync(entity, token);
        }

        public async Task<Tutor> GetTailorEdTutorAsync(CancellationToken token)
        {
            return await Session.Query<Tutor>().Where(w => w.Type == TutorType.TailorEd).FirstAsync(token);
        }
    }
}