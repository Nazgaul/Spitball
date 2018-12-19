using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Application.Models;
using JetBrains.Annotations;

namespace Cloudents.Application.Interfaces
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
