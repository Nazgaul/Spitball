using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Application.Models;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public interface ITutorProvider
    {
        [ItemCanBeNull]
        Task<IEnumerable<TutorDto>> SearchAsync(string term,
            [NotNull]
            TutorRequestFilter[] filters,
            TutorRequestSort sort,
            GeoPoint location,
            int page,
            bool isMobile,
            CancellationToken token);
    }
}
