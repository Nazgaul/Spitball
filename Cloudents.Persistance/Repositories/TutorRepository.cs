using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;

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
    }
}