using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using JetBrains.Annotations;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorSearch
    {
        Task<IEnumerable<TutorDto>> SearchAsync(IEnumerable<string> term,
            TutorRequestFilter[] filters, TutorRequestSort sort,
           [CanBeNull] GeoPoint location, 
            int page,
            bool isMobile,
            CancellationToken token);
    }
}
