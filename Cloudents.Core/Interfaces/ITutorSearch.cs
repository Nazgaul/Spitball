using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorSearch
    {
        Task<IEnumerable<TutorDto>> SearchAsync(string term,
            TutorRequestFilter filter, TutorRequestSort sort,
            GeoPoint location, int page, CancellationToken token);
    }
}
