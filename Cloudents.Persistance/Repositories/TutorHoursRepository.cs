using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class TutorHoursRepository : NHibernateRepository<TutorHours>, ITutorHoursRepository
    {
        public TutorHoursRepository(ISession session) : base(session)
        {
        }

        public async Task<IEnumerable<TutorHours>> GetTutorHoursAsync(long TutorId, CancellationToken token)
        {
            return await Session.Query<TutorHours>().Where(w => w.Tutor.Id == TutorId).ToListAsync(token);
        }
    }
}
