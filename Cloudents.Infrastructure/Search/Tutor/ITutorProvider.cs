using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public interface ITutorProvider
    {
        Task<IEnumerable<TutorDto>> SearchAsync(string term,
            TutorRequestFilter[] filters, TutorRequestSort sort,
            Core.Models.Location location, int page, CancellationToken token);
    }
}
