using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorHoursRepository : IRepository<TutorHours>
    {
        Task<IEnumerable<TutorHours>> GetTutorHoursAsync(long tutorId, CancellationToken token);
    }
}