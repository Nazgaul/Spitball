using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorCalendarRepository : IRepository<TutorCalendar>
    {
        Task<IEnumerable<TutorCalendar>> GetTutorCalendarsAsync(long tutorId, CancellationToken token);
    }
}