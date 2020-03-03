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
    public class TutorCalendarRepository : NHibernateRepository<TutorCalendar>,  ITutorCalendarRepository
    {
        public TutorCalendarRepository(ISession session) : base(session)
        {
        }

        public async Task<IEnumerable<TutorCalendar>> GetTutorCalendarsAsync(long tutorId, CancellationToken token)
        {
            return await Session.Query<TutorCalendar>().Where(w => w.Tutor.Id == tutorId).ToListAsync(token);
        }
    }
}
